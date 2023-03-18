using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NarLib;
using Server.Data;
using Server.Hubs;
using Server.Models;
using System.Net;
using System.Text;

namespace Server.Controllers
{
	[Route("api/client")]
	[ApiController]
	public class ClientController : ControllerBase
	{
		private readonly IClientRepo _clientRepo;
		private readonly IRequestRepo _requestRepo;
		private readonly IWebHostEnvironment _env;
		private readonly IMapper _mapper;
		private readonly IHubContext<GeneralHub> _hubContext;
		private readonly IFileRepo _fileRepo;

		public ClientController(
			IClientRepo serverRepo, IRequestRepo requestRepo, IWebHostEnvironment env, 
			IMapper mapper, IHubContext<GeneralHub> hubContext, IFileRepo fileRepo)
		{
			_clientRepo = serverRepo;
			_requestRepo = requestRepo;
			_env = env;
			_mapper = mapper;
			_hubContext = hubContext;
			_fileRepo = fileRepo;
		}

		[HttpGet]
		public IActionResult Get() => Ok("Service is working");

		[HttpPost]
		public IActionResult Register([FromBody]ClientRegisterDto clientDto)
		{

			var client = _mapper.Map<Client>(clientDto);

			if (_clientRepo.Exists(client))
				return new JsonResult(new RegistrationResponse{ Status = "ERR", ClientId = -1, Secret = "null" });

			_clientRepo.Add(client);
			_clientRepo.SaveChanges();

			return new JsonResult( new RegistrationResponse { Status = "OK", ClientId = client.Id, Secret = client.Secret} );
		}

		[HttpPut]
		public IActionResult UpdateClientData([FromBody] ClientRegisterDto clientDto)
		{
			var client = AuthClient();

			if (client == null)
				return BadRequest("Bad credentials.");

			if (!IPAddress.TryParse(clientDto.LocalIp, out var localIp) || !IPAddress.TryParse(clientDto.PublicIp, out var publicIp))
				return BadRequest("Local IP OR/AND public IP is wrong.");

			client.Os = clientDto.Os;
			client.Hostname = clientDto.Hostname;
			client.LocalIp = localIp;
			client.PublicIp = publicIp;
			client.ProgramVersion = clientDto.ProgramVersion;
			client.ProgramStartUtcTime = clientDto.ProgramStartUtcTime;
			client.UptimeTicks = clientDto.UptimeTicks;
			client.LastUpdateUtcTime = DateTime.UtcNow;
			client.IsOnline = true;
			client.HasCam = clientDto.HasCam;
			client.HasMic = clientDto.HasMic;

			_clientRepo.SaveChanges();

			return Ok();
		}

		[HttpPost("commands/response")]
		public IActionResult GetCommandResponse([FromBody] ControlRequestResult result)
		{
			if (result.Status <= 0)
				return BadRequest("Wrong result data.");

			var client = _clientRepo.Get(result.ClientId);

			if (client == null)
				return BadRequest("Wrong credentials.");

			if(client.Secret != result.ClientSecret || client.Id != result.ClientId)
				return BadRequest("Wrong credentials.");

			if(Utils.NoLogCmds.TryGetValue(result.RequestId, out string noLogCmd))
			{
				switch (noLogCmd)
				{
					case "GetDirData":
						//_hubContext.Clients.All.SendAsync("DirData", result.Result).GetAwaiter().GetResult();
						_hubContext.Clients.All.SendAsync("DirData", result.Result);
						break;
					default:
						break;
				}

				return Ok();
			}


			var request = _requestRepo.Get(result.RequestId);

			if (request == null)
				return BadRequest("Wrong request ID.");

			if((request.ClientId != result.ClientId) || (request.Status == ControlRequestStatus.Succeeded))
				return BadRequest("Wrong request ID.");


			request.Status = result.Status;
			request.Result  = result.Result;
			request.Files = result.Files;

			_requestRepo.SaveChanges();

			string statusString;
			NotificationType type;

			switch (result.Status)
			{
				case ControlRequestStatus.TimedOut:
					type = NotificationType.Warning;
					statusString = "timed out.";
					break;
				case ControlRequestStatus.Succeeded:
					type = NotificationType.Success;
					statusString = "executed successfully.";
					break;
				default:
					type = NotificationType.Danger;
					statusString = "was unsuccessful.";
					break;
			}

			var text = $"{client.Hostname} - request #{request.Id} {statusString}";

			var notification = new NotificationModel() { Text = text, Type = type, Url =  ""};

			_hubContext.Clients.All.SendAsync("TriggerNotification", notification).GetAwaiter().GetResult();

			return Ok();
		}

		[HttpPost("files")]
		[DisableRequestSizeLimit]
		public IActionResult UploadFiles([FromForm]IFormFileCollection files, [FromQuery]bool isWallpaper = false)
		{
			var client = AuthClient();

			if (client == null)
				return BadRequest("Bad credentials.");


			var ticks = DateTime.Now.Ticks;
			var names = new List<string>();

			for (int i = 0; i < files.Count(); i++)
			{
				string ext = "";

				var splittedName = files[i].FileName.Split('.');

				if (splittedName.Length > 1)
					ext = $".{splittedName.Last()}";

				var name = $"{ticks}-{i}{ext}";

				if (isWallpaper & (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".bmp"))
					return BadRequest("Bad wallpaper extension.");
				else if(isWallpaper)
					name = $"Client{client.Id}-wallpaper{ext}";


				names.Add(name);

				var path = $"{_env.ContentRootPath}\\ClientApp\\public\\Clients\\Files\\{name}";

				using (var fs = new FileStream(path, FileMode.OpenOrCreate))
				{
					files[i].CopyTo(fs);
					fs.Close();
				}

				var dbFile = new UploadedFile() 
				{ 
					ClientId = client.Id, 
					Filename = name, 
					OriginalFilename = files[i].FileName,  
				};

				_fileRepo.Add(dbFile);
				_fileRepo.SaveChanges();

				if (isWallpaper)
				{
					client.WallpaperPath = $"\\Clients\\Files\\{name}";
					_clientRepo.SaveChanges();

					return Ok(new[] { name });
				}

			}

			return Ok(names.ToArray());
		}

		public class AuthModel
		{
			public int Id { get; set; }
			public string Secret { get; set; } = "";
		}

		[NonAction]
		public AuthModel? GetCreds()
		{
			var header = HttpContext.Request.Headers.Authorization.ToString();

			try
			{
				if (header != null && header.StartsWith("Basic"))
				{
					var encodedString = header.Substring("Basic ".Length).Trim();
					var decodedString = Encoding.ASCII.GetString(Convert.FromBase64String(encodedString));

					var authArray = decodedString.Split(':');
					if (authArray.Length != 2)
						return null;

					if (!int.TryParse(authArray[0], out var id))
						return null;


					return new AuthModel() { Id = id, Secret = authArray[1] };
				}
			}
			catch { }

			return null;
		}

		[NonAction]
		public Client? AuthClient()
		{
			var creds = GetCreds();

			if (creds == null)
				return null;

			var client = _clientRepo.Get(creds.Id);

			if (client == null)
				return null;

			if (client.Secret != creds.Secret)
				return null;

			return client;
		}
	}
}

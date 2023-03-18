using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Server.Data;

namespace Server.Controllers
{
	[Route("api/logs")]
	[ApiController]
	public class LogsController : ControllerBase
	{
		private readonly IClientRepo _clientRepo;
		private readonly IRequestRepo _requestRepo;
		private readonly IWebHostEnvironment _env;
		private readonly IMapper _mapper;
		public LogsController(IClientRepo clientRepo, IRequestRepo requestRepo, IWebHostEnvironment env, IMapper mapper)
		{
			_clientRepo = clientRepo;
			_requestRepo = requestRepo;
			_env = env;
			_mapper = mapper;
		}

		[HttpGet]
		public IActionResult GetRequestLogs()
		{
			var indexString = HttpContext.Request.Query["skip"].ToString();
			var countString = HttpContext.Request.Query["count"].ToString();
			var clientIdString = HttpContext.Request.Query["clientId"].ToString();

			if (!int.TryParse(indexString, out var index))
				index = 0;

			if (!int.TryParse(countString, out var count))
				count = 100;

			if (!int.TryParse(clientIdString, out var clientId))
				clientId = -1;

			var logs = _requestRepo.GetAll().Reverse().Skip(index);

			if (clientId != -1)
				logs = logs.Where(e => e.Id == clientId);

			var a = logs.Count();

			var result = logs.Take(count).ToList();

			return Ok(result);
		}
	}
}

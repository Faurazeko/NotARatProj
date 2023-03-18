using AForge.Video.DirectShow;
using NarLib;
using System.Management;
using System.Net;
using System.Net.Sockets;

namespace ClientWF
{
	public static class PcInfoGetter
	{
		public static ClientRegisterDto GetFullData()
		{

			var model = new ClientRegisterDto()
			{
				Os = GetSystemName(),
				Hostname = Dns.GetHostName(),
				LocalIp = GetLocalIp(),
				PublicIp = GetPublicIp(),
				ProgramVersion = ConfigManager.Config.ProgramVersion,
				ProgramStartUtcTime = Program.UtcStartTime,
				UptimeTicks = Environment.TickCount,
				HasCam = GetCams().Count > 0,
				HasMic = GetMics().Count > 0
			};

			return model;
		}

		public static string GetSystemName()
		{
			var name = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
						select x.GetPropertyValue("Caption")).FirstOrDefault();

			return name == null ? "Unknown" : name.ToString()!;
		}

		public static string GetLocalIp()
		{
			string localIP;
			using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
			{
				socket.Connect("8.8.8.8", 65530);
				IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
				localIP = endPoint.Address.ToString();
			}

			return localIP;
		}

		public static string GetPublicIp()
		{
			string responseText = new WebClient().DownloadString("http://icanhazip.com");
			var ipString = responseText.Replace("\\r\\n", "").Replace("\\n", "").Trim();

			return ipString;
		}

		public static FilterInfoCollection GetCams() => new FilterInfoCollection(FilterCategory.VideoInputDevice);

		public static FilterInfoCollection GetMics() => new FilterInfoCollection(FilterCategory.AudioInputDevice);
	}
}

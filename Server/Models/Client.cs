using NarLib;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace Server.Models
{
	public class Client
	{
		[Key]
		public int Id { get; set; }
		[JsonIgnore]
		public string Secret { get; set; } = Guid.NewGuid().ToString();
		public bool IsOnline { get; set; } = true;
		public string Os { get; set; }
		public string Hostname { get; set; }
		[JsonIgnore]
		public IPAddress LocalIp { get; set; }
		[JsonIgnore]
		public IPAddress PublicIp { get; set; }
		public string ProgramVersion { get; set; }
		public string WallpaperPath { get; set; } = "/Default/wallpaper.png";

		[DataType("datetime2")]
		public DateTime LastUpdateUtcTime { get; set; } = DateTime.UtcNow;
		[DataType("datetime2")]
		public DateTime? ProgramStartUtcTime { get; set; } = DateTime.UtcNow;
		public int UptimeTicks { get; set; } = 0;

		[JsonIgnore]
		public List<ControlRequest> ControlRequests { get; set; } = new();


		public bool HasCam { get; set; }
		public bool HasMic { get; set; }
	}
}

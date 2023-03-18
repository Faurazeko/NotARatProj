namespace Server.Dtos
{
	public class ClientMngDto
	{
		public int Id { get; set; }
		public bool IsOnline { get; set; } = true;
		public string Os { get; set; }
		public string Hostname { get; set; }
		public string LocalIp { get; set; }
		public string PublicIp { get; set; }
		public string ProgramVersion { get; set; }
		public string WallpaperPath { get; set; }

		public DateTime LastUpdateUtcTime { get; set; } = DateTime.UtcNow;
		public DateTime ProgramStartUtcTime { get; set; } = DateTime.UtcNow;
		public int UptimeTicks { get; set; } = 0;

		public bool HasCam { get; set; }
		public bool HasMic { get; set; }
	}
}

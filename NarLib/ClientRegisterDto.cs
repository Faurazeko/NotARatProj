namespace NarLib
{
    public class ClientRegisterDto
    {
        public string Os { get; set; } = "Unknown";
		public string Hostname { get; set; } = "Unknown";
		public string? LocalIp { get; set; }
        public string? PublicIp { get; set; }
        public string ProgramVersion { get; set; } = "Unknown";

        public DateTime ProgramStartUtcTime { get; set; } = DateTime.UtcNow;
        public int UptimeTicks { get; set; } = 0;

        public bool HasCam { get; set; } = false;
        public bool HasMic { get; set; } = false;
	}
}

namespace ClientWF.Models
{
    public class Config
    {
        public string ProgramVersion { get; set; } = "Unknown";
        public DateTime? FirstRegistrationUtcTime { get; set; }
        public bool IsRegistred { get; set; } = false;
        public int ClientId { get; set; } = -1;
        public string ClientSecret { get; set; } = string.Empty;
        public string ServerIp { get; set; } = "127.0.0.1";
        public int ServerTcpPort { get; set; } = 13370;
        public int ServerHttpPort { get; set; } = 44405;
    }
}

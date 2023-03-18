using NarLib;

namespace Server.Models
{
	public class ControlModel
	{
		public string Command { get; set; }
		public bool ForceOfflineSend { get; set; }
		public ControlRequestType Type { get; set; }
	}
}

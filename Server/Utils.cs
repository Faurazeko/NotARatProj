namespace Server
{
	public static class Utils
	{
		private static Dictionary<int, string> _noLogCmds =
		new()
		{
			{ -1, "GetDirData"}
		};

		public static Dictionary<int, string> NoLogCmds 
		{ 
			get => _noLogCmds;
		}
	}
}

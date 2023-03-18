namespace ClientWF
{
	public static class Logger
	{
		private static string logPath = "log.log";

		public static void Log(object obj) => Console.WriteLine(obj.ToString());
	}
}

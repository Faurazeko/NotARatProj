using ClientWF.Models;

namespace ClientWF
{
	class ConfigManager
	{
		public static Config Config = null;
		private static string _currentVersionName = "0.0.69";
		private static string _configPath = "config.json";

		public static void LoadConfig()
		{
			if (!File.Exists(_configPath))
			{
				_createDefaultConfig();
				return;
			}
			else
			{
				var configText = File.ReadAllText(_configPath);
				var config = System.Text.Json.JsonSerializer.Deserialize<Config>(configText);

				if (config == null)
				{
					_createDefaultConfig();
					return;
				}

				Config = config;

				return;
			}
		}

		public static void SaveConfig()
		{
			var text = System.Text.Json.JsonSerializer.Serialize(Config);
			File.WriteAllText(_configPath, text);
		}

		public static void ResetConfig() => _createDefaultConfig();

		private static void _createDefaultConfig()
		{
			var config = new Config();
			config.ProgramVersion= _currentVersionName;


			Config = config;
			SaveConfig();
		}
	}
}

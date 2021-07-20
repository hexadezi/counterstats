using System.IO;
using System.Text.Json;

namespace counterstats
{
	public class Settings
	{
		public string CsgoPath { get; set; } = "";
		public string ApiKey { get; set; } = "";
		public string MySteamID { get; set; } = "";
		public bool IgnoreFriends { get; set; }
		public bool IgnoreOwnId { get; set; }
		public bool OnTop { get; set; }
	}


	public static class SettingsProvider
	{
		public static Settings Settings { get; set; }
		public static string Savefile { get; set; } = "config.json";

		static SettingsProvider()
		{
			if (!File.Exists(Savefile))
			{
				Settings = new Settings();
				Save();
			}
			if (Settings == null)
			{
				Load();
				Save();
			}
		}

		public static void Save()
		{
			JsonSerializerOptions options = new()
			{
				WriteIndented = true
			};
			string jsonString = JsonSerializer.Serialize(Settings, options);
			File.WriteAllText(Savefile, jsonString);
		}

		public static void Load()
		{
			string jsonString = File.ReadAllText(Savefile);
			Settings = JsonSerializer.Deserialize<Settings>(jsonString);
		}
	}
}

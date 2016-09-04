using GO_Bot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;
using System.Runtime.Serialization.Formatters;

namespace GO_Bot.Internals {

	internal static class Settings {

		private static string GeneralSettingsFileName = ApplicationEnvironment.SettingsDirectory() + @"\GeneralSettings.json";
		private static string MainWindowModelFileName = ApplicationEnvironment.SettingsDirectory() + @"\MainWindowModel.json";

		public static GeneralSettings GeneralSettings { get; private set; }
		public static MainWindowModel MainWindowModel { get; private set; }

		public static void Load() {
			GeneralSettings = new GeneralSettings();
			MainWindowModel = new MainWindowModel();
			JsonSerializerSettings settings = new JsonSerializerSettings() {
				Formatting = Formatting.Indented,
				TypeNameHandling = TypeNameHandling.Auto,
				TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple,
				ObjectCreationHandling = ObjectCreationHandling.Replace
			};
			settings.Converters.Add(new StringEnumConverter() { CamelCaseText = true });

			if (File.Exists(GeneralSettingsFileName)) {
				GeneralSettings = JsonConvert.DeserializeObject<GeneralSettings>(File.ReadAllText(GeneralSettingsFileName), settings) ?? GeneralSettings;
			}

			if (File.Exists(MainWindowModelFileName)) {
				MainWindowModel = JsonConvert.DeserializeObject<MainWindowModel>(File.ReadAllText(MainWindowModelFileName), settings) ?? MainWindowModel;
			}

			Save(); // Ensure settings files get created and are up to date
		}

		public static void Save() {
			File.WriteAllText(GeneralSettingsFileName, JsonConvert.SerializeObject(GeneralSettings, Formatting.Indented));
			File.WriteAllText(MainWindowModelFileName, JsonConvert.SerializeObject(MainWindowModel, Formatting.Indented));
		}

	}
	
	[JsonObject(MemberSerialization = MemberSerialization.OptOut)]
	internal class GeneralSettings {

		public double WindowLeft;
		public double WindowTop;
		public double WindowWidth;
		public double WindowHeight;

	}

}

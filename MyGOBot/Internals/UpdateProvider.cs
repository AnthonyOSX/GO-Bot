using Newtonsoft.Json;
using NLog;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace GO_Bot.Internals {

	internal static class UpdateProvider {

		private static Logger logger = LogManager.GetCurrentClassLogger();

		public static Task CheckForUpdates(string url) {
			return Task.Run(async () => {
				try {
					logger.Info("Checking for updates...");

					HttpResponseMessage response = await HttpProvider.Get(url);

					if (response.IsSuccessStatusCode) {
						UpdateJson json = JsonConvert.DeserializeObject<UpdateJson>(await response.Content.ReadAsStringAsync());
						Version serverVersion = new Version(json.Version);

						if (serverVersion.CompareTo(Assembly.GetExecutingAssembly().GetName().Version) > 0) {
							if (MethodProvider.DisplayMessage("Update", $"There is an application update available ({serverVersion}). Download now?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
								Process.Start(json.DownloadUrl);
							} else {
								logger.Info("You are running an outdated version of this application!");
							}
						} else {
							logger.Info("You're running the latest application version!");
						}
					} else {
						logger.Error($"Failed checking for updates ({(int)response.StatusCode} {response.StatusCode})");
					}
				} catch (Exception e) {
					logger.Error(e, $"Unhandled exception while checking for updates ({e.InnerException.Message})");
				}
			});
		}

	}

	internal class UpdateJson {

		[JsonProperty(PropertyName = "version")]
		public string Version { get; set; }

		[JsonProperty(PropertyName = "download_url")]
		public string DownloadUrl { get; set; }

	}

}

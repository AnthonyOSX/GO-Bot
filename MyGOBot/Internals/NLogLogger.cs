using System;
using Logger = NLog.Logger;
using MyGOBot.Logic.Logging;

namespace GO_Bot.Internals {

	internal class NLogLogger : ILogger {

		private static Logger logger = NLog.LogManager.GetLogger("API");

		public void Write(string message, LogLevel level = LogLevel.Info) {
			logger.Info(message);
		}

		public void Write(string message, LogLevel level = LogLevel.Info, ConsoleColor color = ConsoleColor.Black) {
			Write(message, level);
		}

	}

}

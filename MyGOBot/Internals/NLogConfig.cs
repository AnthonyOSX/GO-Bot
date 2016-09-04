using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace GO_Bot.Internals {

	internal static class NLogConfig {

		public static string BaseLogDirectory { get; private set; }

		public static bool IsOutputLoggingEnabled { get; private set; }
		public static bool IsWarningLoggingEnabled { get; private set; }
		public static bool IsErrorLoggingEnabled { get; private set; }
		public static bool IsDebugConsoleOutputEnabled { get; private set; }

		public static void Init() {
			BaseLogDirectory = ApplicationEnvironment.LogsDirectory();
			LogManager.ThrowExceptions = false;
			InternalLogger.LogToConsole = false;
			InternalLogger.LogLevel = LogLevel.Error;
			LogManager.Configuration = new LoggingConfiguration();
			EnableOutputLogging();
			EnableWarningLogging();
			EnableErrorLogging();

#if DEBUG
			EnableDebugConsoleOutput();
#endif

			LogManager.ReconfigExistingLoggers();
		}

		public static void EnableOutputLogging() {
			if (IsOutputLoggingEnabled) {
				return;
			}

			LoggingConfiguration config = LogManager.Configuration;
			FileTarget outputTarget = new FileTarget() {
				Name = "fileOutput",
				Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message}",
				FileName = BaseLogDirectory + @"\Output\output.txt",
				ArchiveFileName = BaseLogDirectory + @"\Output\output.${shortdate}-{#}.txt",
				ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
				ArchiveAboveSize = 10485760,
				MaxArchiveFiles = 10,
				KeepFileOpen = true,
				ConcurrentWrites = true
			};
			ConsoleTarget consoleTarget = new ConsoleTarget() {
				Name = "consoleOutput",
				Layout = @"${date:format=yyyy-MM-dd hh\:mm\:ss tt} | ${level:uppercase=true} | ${message}",
			};

			config.AddTarget(outputTarget);
			config.AddTarget(consoleTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, outputTarget));
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Info, consoleTarget));
			IsOutputLoggingEnabled = true;
		}

		public static void EnableWarningLogging() {
			if (IsWarningLoggingEnabled) {
				return;
			}

			LoggingConfiguration config = LogManager.Configuration;
			FileTarget warningTarget = new FileTarget() {
				Name = "fileWarnings",
				Layout = "${longdate} | ${level:uppercase=true} | ${logger} | ${message}",
				FileName = BaseLogDirectory + @"\Warning\warning.txt",
				ArchiveFileName = BaseLogDirectory + @"\Warning\warning.${shortdate}-{#}.txt",
				ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
				ArchiveAboveSize = 10485760,
				MaxArchiveFiles = 10,
				KeepFileOpen = true,
				ConcurrentWrites = true
			};
			LoggingRule warningRule = new LoggingRule("*", warningTarget);

			warningRule.EnableLoggingForLevel(LogLevel.Warn);
			config.AddTarget(warningTarget);
			config.LoggingRules.Add(warningRule);
			IsWarningLoggingEnabled = true;
		}

		public static void EnableErrorLogging() {
			if (IsErrorLoggingEnabled) {
				return;
			}

			LoggingConfiguration config = LogManager.Configuration;
			FileTarget errorTarget = new FileTarget() {
				Name = "fileError",
				Layout = "-------------- ${level} (${longdate}) --------------${newline}"
					+ "${newline}"
					+ "Call Site: ${callsite}${newline}"
					+ "Type: ${exception:format=Type}${newline}"
					+ "Message: ${exception:format=Message}${newline}"
					+ "Stack Trace: ${exception:format=StackTrace}${newline}"
					+ "${newline}",
				FileName = BaseLogDirectory + @"\Error\error.txt",
				ArchiveFileName = BaseLogDirectory + @"\Error\error.${shortdate}-{#}.txt",
				ArchiveNumbering = ArchiveNumberingMode.DateAndSequence,
				ArchiveAboveSize = 10485760,
				MaxArchiveFiles = 10,
				KeepFileOpen = true,
				ConcurrentWrites = true
			};

			config.AddTarget(errorTarget);
			config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, errorTarget));
			IsErrorLoggingEnabled = true;
		}

		public static void EnableDebugConsoleOutput() {
			if (IsDebugConsoleOutputEnabled) {
				return;
			}

			LoggingConfiguration config = LogManager.Configuration;
			ConsoleTarget consoleTarget = new ConsoleTarget() { Name = "consoleDebugOutput" };
			LoggingRule consoleRule = new LoggingRule("*", consoleTarget);

			consoleRule.EnableLoggingForLevel(LogLevel.Trace);
			consoleRule.EnableLoggingForLevel(LogLevel.Debug);
			config.AddTarget(consoleTarget);
			config.LoggingRules.Add(consoleRule);
			IsDebugConsoleOutputEnabled = true;
		}

	}

}

using System;
using System.Runtime.CompilerServices;

namespace MT.Toolkit.LogTool
{
	public class SimpleLogger
	{
		static Logger logger;
		public static void Config(Action<SimpleLoggerConfiguration> action)
		{
			var logConfig = new SimpleLoggerConfiguration();
			action?.Invoke(logConfig);
			logger = new Logger(logConfig);
		}

		static void CheckLoggerConfig()
		{
			if (logger == null) throw new Exception("SimpleLogger未初始化，请调用 SimpleLogger.Config");
		}

		public static void LogInformation(string msg,
			[CallerFilePath] string category = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = "",
			Exception ex = null)
		{
			CheckLoggerConfig();
			logger.Log(SimpleLogLevel.Information, msg, (s, e) => s, category, eventId: line, eventName: member, exception: ex);
		}

		public static void LogDebug(string msg,
			[CallerFilePath] string category = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = "",
			Exception ex = null)
		{
			CheckLoggerConfig();
			logger.Log(SimpleLogLevel.Debug, msg, (s, e) => s, category, eventId: line, eventName: member, exception: ex);
		}

		public static void LogError(string msg,
			[CallerFilePath] string category = "",
			[CallerLineNumber] int line = 0,
			[CallerMemberName] string member = "",
			Exception ex = null)
		{
			CheckLoggerConfig();
			logger.Log(SimpleLogLevel.Error, msg, (s, e) => s, category, eventId: line, eventName: member, exception: ex);
		}
	}
}

using System;
using System.Collections.Generic;

namespace MT.Toolkit.LogTool
{
	public class SimpleLoggerConfiguration
	{
		public string LogDirectory { get; set; }

		public Dictionary<SimpleLogLevel, LogType> LogTarget { get; set; } = new Dictionary<SimpleLogLevel, LogType>
		{
			[SimpleLogLevel.Trace] = LogType.Console,
			[SimpleLogLevel.Information] = LogType.Console,
			[SimpleLogLevel.Warning] = LogType.Console,
			[SimpleLogLevel.Debug] = LogType.Console | LogType.File | LogType.Debug,
			[SimpleLogLevel.Error] = LogType.Console | LogType.File,
			[SimpleLogLevel.Critical] = LogType.Console | LogType.File,
			[SimpleLogLevel.None] = LogType.Console,
		};

		public Dictionary<SimpleLogLevel, ConsoleColor> ConsoleColor { get; set; } = new Dictionary<SimpleLogLevel, ConsoleColor>
		{
			[SimpleLogLevel.Information] = System.ConsoleColor.Green,
			[SimpleLogLevel.Trace] = System.ConsoleColor.DarkBlue,
			[SimpleLogLevel.Warning] = System.ConsoleColor.Yellow,
			[SimpleLogLevel.Debug] = System.ConsoleColor.DarkBlue,
			[SimpleLogLevel.Error] = System.ConsoleColor.Red,
			[SimpleLogLevel.Critical] = System.ConsoleColor.Red,
			[SimpleLogLevel.None] = System.ConsoleColor.Gray,
		};

		public Action<LogInfo> LogActionIntercept { get; set; }

		public LogType EnabledLogType { get; set; } = LogType.Console;

		/// <summary>
		/// 启用所有默认日志 Console | Debug | File
		/// </summary>
		public void EnableAllDefault()
		{
			EnabledLogType = LogType.Console | LogType.Debug | LogType.File;
		}
		internal Func<ISimpleLogger> CustomLogger { get; set; }
		public void EnableCustomLogger(Func<ISimpleLogger> getLogger)
		{
			CustomLogger = getLogger;
			EnabledLogType = EnabledLogType | LogType.Custom;
		}

		public void RedirectLogTarget(SimpleLogLevel logLevel, LogType logType)
		{
			LogTarget[logLevel] = logType;
		}

	}
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MT.Toolkit.LogTool
{
	public class SimpleLoggerConfiguration
	{
		public string LogDirectory { get; set; }

		public Dictionary<LogLevel, LogType> LogTarget { get; set; } = new Dictionary<LogLevel, LogType>
		{
			[LogLevel.Trace] = LogType.Console,
			[LogLevel.Information] = LogType.Console,
			[LogLevel.Warning] = LogType.Console | LogType.File,
			[LogLevel.Debug] = LogType.Console | LogType.File | LogType.Debug,
			[LogLevel.Error] = LogType.Console | LogType.File,
			[LogLevel.Critical] = LogType.Console | LogType.File,
			[LogLevel.None] = LogType.Console,
		};

		public Dictionary<LogLevel, ConsoleColor> ConsoleColor { get; set; } = new Dictionary<LogLevel, ConsoleColor>
		{
			[LogLevel.Information] = System.ConsoleColor.Green,
			[LogLevel.Trace] = System.ConsoleColor.DarkBlue,
			[LogLevel.Warning] = System.ConsoleColor.Yellow,
			[LogLevel.Debug] = System.ConsoleColor.DarkBlue,
			[LogLevel.Error] = System.ConsoleColor.Red,
			[LogLevel.Critical] = System.ConsoleColor.Red,
			[LogLevel.None] = System.ConsoleColor.Gray,
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

		public void RedirectLogTarget(LogLevel logLevel, LogType logType)
		{
			LogTarget[logLevel] = logType;
		}

	}
}

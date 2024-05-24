using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace MT.Toolkit.LogTool
{
	internal class Logger
	{
		internal Dictionary<LogType, ISimpleLogger> LoggerDict { get; } = new Dictionary<LogType, ISimpleLogger>();
		private SimpleLoggerConfiguration logConfig;

		public Logger(SimpleLoggerConfiguration config)
		{
			logConfig = config;
			Enable(config.EnabledLogType);
		}

		public void Enable(LogType logType)
		{
			if (logType.HasFlag(LogType.Console))
			{
				LoggerDict[LogType.Console] = new ConsoleLogger();
			}
			if (logType.HasFlag(LogType.Debug))
			{
				LoggerDict[LogType.Debug] = new DebugLogger();
			}
			if (logType.HasFlag(LogType.File))
			{
				LoggerDict[LogType.File] = LocalFileLogger.Instance;
			}
			if (logType.HasFlag(LogType.Custom))
			{
				LoggerDict[LogType.Custom] = logConfig.CustomLogger?.Invoke() ?? throw new Exception("未设置自定义日志对象");
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log<TState>(LogLevel logLevel, TState state, Func<TState, Exception, string> formatter, string category = "", int eventId = 0, string eventName = "", Exception exception = null)
		{
			var logInfo = new LogInfo<TState>()
			{
				LogLevel = logLevel,
				Message = formatter.Invoke(state, exception),
				EventId = eventId,
				EventName = eventName,
				Category = category,
				Exception = exception
			};
			Write<TState>(logLevel, logInfo);
		}

		object locker = new object();
		public void Write<T>(LogLevel level, LogInfo logInfo)
		{
			lock (locker)
			{
				logConfig.LogActionIntercept?.Invoke(logInfo);

				if (logInfo.Handled)
				{
					return;
				}

				if (!logConfig.LogTarget.TryGetValue(level, out var logType))
				{
					return;
				}
				foreach (var item in LoggerDict)
				{
					if (!logType.HasFlag(item.Key))
					{
						continue;
					}
					item.Value.LogConfig = logConfig;
					item.Value.WriteLog(logInfo);
				}
			}
		}
	}
}

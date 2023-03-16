using System;

namespace MT.Toolkit.LogTool
{
	public class LogInfo
	{
		/// <summary>
		/// 时间
		/// </summary>
		public DateTime Time { get; set; } = DateTime.Now;
		/// <summary>
		/// 线程id
		/// </summary>
		public int ThreadId { get; set; }
		/// <summary>
		/// 日志级别
		/// </summary>
		public SimpleLogLevel LogLevel { get; set; }
		/// <summary>
		/// 异常信息
		/// </summary>
		public string Message { get; set; }

		/// <summary>
		/// 异常对象
		/// </summary>
		public Exception Exception { get; set; }
		public int EventId { get; set; }
		public string EventName { get; set; }
		public string Category { get; set; }

		public object State { get; set; }
		public string Source { get; set; }
		/// <summary>
		/// 触发日志的行数
		/// </summary>
		public int LogLine { get; set; }
		/// <summary>
		/// 触发日志的方法
		/// </summary>
		public string LogMember { get; set; }
		public bool Handled { get; set; }
	}
	/// <summary>
	/// 日志信息
	/// </summary>
	public class LogInfo<TState> : LogInfo
	{
		public new TState State { get; set; }
	}

	public static class LogInfoExtensions
	{
		public static string FormatLogMessage(this LogInfo self)
		{
			//[TId:{self.ThreadId.ToString().PadLeft(5, '0')}] 
			return $"{self.LogHeader()}{self.LogCategory()}{self.LogBody()}";
		}

		public static string LogHeader(this LogInfo self)
		{
			return $"[{self.Time:yyyy-MM-dd HH:mm:ss}][{self.LogLevel}]:";
		}
		public static string LogCategory(this LogInfo self)
		{
			return $"{self.Category}[{self.EventId}]{Environment.NewLine}";
		}
		public static string LogBody(this LogInfo self)
		{
			return $"    {self.LogException()}{self.LogTrace()}";
		}

		public static string LogException(this LogInfo self)
		{
			return $"{self.Message}{(self.Exception == null ? "" : Environment.NewLine)}";
		}

		public static string LogTrace(this LogInfo self)
		{
			return $"{self.Exception?.StackTrace}{Environment.NewLine}";
		}
	}
}

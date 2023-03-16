using System;

namespace MT.Toolkit.LogTool
{
	[Flags]
	public enum LogType
	{
		Console = 1,
		File = 1 << 1,
		Debug = 1 << 2,
		Custom = 1 << 3,
	}
	/// <summary>
	/// 日志级别
	/// </summary>
	public enum SimpleLogLevel
	{
		Trace,
		/// <summary>
		/// 信息级别
		/// </summary>
		Information,
		/// <summary>
		/// debug级别
		/// </summary>
		Debug,
		/// <summary>
		/// 警告级别
		/// </summary>
		Warning,
		/// <summary>
		/// 错误级别
		/// </summary>
		Error,
		/// <summary>
		/// 致命级别
		/// </summary>
		Critical,
		None,
	}

}

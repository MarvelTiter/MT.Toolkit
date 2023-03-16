using System;
using System.Diagnostics;

namespace MT.Toolkit.LogTool
{
	public class DebugLogger : ISimpleLogger
	{
		public SimpleLoggerConfiguration LogConfig { get; set; }

		public void WriteLog(LogInfo logInfo)
		{
			Debug.Write(logInfo.LogHeader());
			Debug.Write(logInfo.LogCategory());
			Debug.Write(logInfo.LogBody());
		}
	}
}

using System;

namespace MT.Toolkit.LogTool
{
	public interface ISimpleLogger
	{
		SimpleLoggerConfiguration LogConfig { get; set; }
		void WriteLog(LogInfo logInfo);
	}
}

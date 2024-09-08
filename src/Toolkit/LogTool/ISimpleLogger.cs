using Microsoft.Extensions.Logging;
using System;

namespace MT.Toolkit.LogTool
{
	public interface ISimpleLogger
	{
		//LoggerSetting LogConfig { get; set; }
        void WriteLog(LogInfo logInfo);
	}
}

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace MT.Toolkit.LogTool
{
	public class DebugLogger : ISimpleLogger
	{
		public LoggerSetting LogConfig { get; set; }

        public DebugLogger(LoggerSetting configuration)
        {
            LogConfig = configuration;
        }
        
        public void WriteLog(LogInfo logInfo)
		{
			Debug.Write(logInfo.LogHeader());
			Debug.Write(logInfo.LogCategory());
			Debug.Write(logInfo.LogBody());
		}
	}
}

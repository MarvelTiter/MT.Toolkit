using Microsoft.Extensions.Logging;
using System;

namespace MT.Toolkit.LogTool;
#pragma warning disable
[Obsolete("please use LoggerProviderExtensions instead.")]
public interface ISimpleLogger
{
    //LoggerSetting LogConfig { get; set; }
    void WriteLog(LogInfo logInfo);
}

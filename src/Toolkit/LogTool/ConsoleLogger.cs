using Microsoft.Extensions.Logging;
using System;

namespace MT.Toolkit.LogTool;
#pragma warning disable
[Obsolete("ConsoleLogger is deprecated, please use LoggerProviderExtensions instead.")]
public class ConsoleLogger : ISimpleLogger
{
    public ConsoleLogger(LoggerSetting configuration)
    {
        LogConfig = configuration;
    }
    public LoggerSetting LogConfig { get; set; }

    
    public void WriteLog(LogInfo logInfo)
    {
        if (!LogConfig.ConsoleColor.TryGetValue(logInfo.LogLevel, out var color))
        {
            color = ConsoleColor.White;
        }
        Console.ForegroundColor = color;
        Console.Write(logInfo.LogHeader());
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(logInfo.LogCategory());
        Console.Write(logInfo.LogBody());
    }
}

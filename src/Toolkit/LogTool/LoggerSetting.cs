using Microsoft.Extensions.Logging;
using MT.Toolkit.LogTool.DbLogger;
using MT.Toolkit.LogTool.FileLogger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
namespace MT.Toolkit.LogTool
{
    public static class LoggerSettingExtensions
    {
        public static bool IsEnabled(this LoggerSetting loggerSetting, LogType logType, LogLevel logLevel)
        {
            return logLevel >= loggerSetting.LogLimit[logType];
        }
        public static void EnableCustomLogger(this LoggerSetting loggerSetting, Func<LoggerSetting, ISimpleLogger> getLogger)
        {
            loggerSetting.CustomLogger = getLogger;
            loggerSetting.EnabledLogType |= LogType.Custom;
        }
        /// <summary>
        /// 启用默认日志 Console | Debug | File
        /// </summary>
        public static void EnableDefaults(this LoggerSetting loggerSetting)
        {
            loggerSetting.EnabledLogType = LogType.Console | LogType.Debug | LogType.File;
        }

        public static void ConfiguraLogLevel(this LoggerSetting loggerSetting, LogType logType, LogLevel logLevel)
        {
            loggerSetting.LogLimit[logType] = logLevel;
        }
    }
    public class LoggerSetting : IFileLoggerSetting, IDbLoggerSetting
    {
        private static readonly Lazy<LoggerSetting> setting = new(() => new());
        public static LoggerSetting Default => setting.Value;
        public string? LogDirectory { get; set; }

        #region FileLogger 
        public int FileSavedDays { get; set; } = 7;
        public string? LogFileFolder { get; set; }
        public long LogFileSize { get; set; } = 1 * 1024 * 1024;
        internal Func<LogInfo, bool> FileLogInfoFilter { get; set; } = _ => true;
        public void SetFileWriteLevel(LogLevel logLevel) => LogLimit[LogType.File] = logLevel;
        public void SetFileLogInfoFilter(Func<LogInfo, bool> filter)
        {
            FileLogInfoFilter = filter;
        }
        #endregion

        #region DbLogger
        internal Func<LogInfo, bool> DbLogInfoFilter { get; set; } = _ => true;
        internal Func<IDbLogger>? DbLoggerFacotry { get; set; }
        public void SetDbWriteLevel(LogLevel logLevel) => LogLimit[LogType.Database] = logLevel;
        public void SetDbLogInfoFilter(Func<LogInfo, bool> filter)
        {
            DbLogInfoFilter = filter;
        }
        public void SetDbLogger(Func<IDbLogger> factory)
        {
            DbLoggerFacotry = factory;
        }
        
        #endregion

        //internal Dictionary<LogLevel, LogType> LogTarget { get; set; } = new Dictionary<LogLevel, LogType>
        //{
        //    [LogLevel.Trace] = LogType.Console,
        //    [LogLevel.Information] = LogType.Console,
        //    [LogLevel.Warning] = LogType.Console | LogType.File,
        //    [LogLevel.Debug] = LogType.Console | LogType.File | LogType.Debug,
        //    [LogLevel.Error] = LogType.Console | LogType.File,
        //    [LogLevel.Critical] = LogType.Console | LogType.File,
        //    [LogLevel.None] = LogType.Console,
        //};

        internal Dictionary<LogType, LogLevel> LogLimit { get; set; } = new()
        {
            [LogType.Debug] = LogLevel.Information,
            [LogType.Console] = LogLevel.Information,
            [LogType.File] = LogLevel.Error,
            [LogType.Database] = LogLevel.Error,
            [LogType.Custom] = LogLevel.None,
        };

        public Dictionary<LogLevel, ConsoleColor> ConsoleColor { get; set; } = new()
        {
            [LogLevel.Information] = System.ConsoleColor.Green,
            [LogLevel.Trace] = System.ConsoleColor.DarkBlue,
            [LogLevel.Warning] = System.ConsoleColor.Yellow,
            [LogLevel.Debug] = System.ConsoleColor.DarkBlue,
            [LogLevel.Error] = System.ConsoleColor.Red,
            [LogLevel.Critical] = System.ConsoleColor.Red,
            [LogLevel.None] = System.ConsoleColor.Gray,
        };

        public Action<LogInfo>? LogActionIntercept { get; set; }

        public LogType EnabledLogType { get; set; } = LogType.Console;

        internal Func<LoggerSetting, ISimpleLogger>? CustomLogger { get; set; }
    }
}

using Microsoft.Extensions.Logging;
using MT.Toolkit.LogTool.DbLogger;
using MT.Toolkit.LogTool.FileLogger;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释

namespace MT.Toolkit.LogTool
{
    public static class LoggerSettingExtensions
    {
        public static bool IsEnabled(this LoggerSetting loggerSetting, LogType logType, string? category, LogLevel logLevel)
        {
            var info = loggerSetting.LogLimit[logType];
            return logLevel >= info.GetLevel(category);
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
        public static void ConfiguraLogLevel<T>(this LoggerSetting loggerSetting, LogType logType, LogLevel logLevel)
        {
            ConfiguraLogLevel(loggerSetting, logType, typeof(T).FullName!, logLevel);
        }
        public static void ConfiguraLogLevel(this LoggerSetting loggerSetting, LogType logType, string category, LogLevel logLevel)
        {
            var info = loggerSetting.LogLimit[logType];
            info.Add(category, logLevel);
        }

        public static void AddOrUpdate(this LoggerSetting loggerSetting, LogType logType, string category, LogLevel logLevel)
        {
            var info = loggerSetting.LogLimit[logType];
            if (info.ContainsKey(category)) info.Remove(category);
            info.Add(category, logLevel);
        }

        public static LogLevel GetLogLevel(this LoggerSetting loggerSetting, LogType logType, string category)
        {
            var info = loggerSetting.LogLimit[logType];
            return info.GetLevel(category);
        }
        public static void SetFileWriteLevel<T>(this IFileLoggerSetting loggerSetting, LogLevel logLevel)
        {
            loggerSetting.SetFileWriteLevel(typeof(T).FullName!, logLevel);
        }
        public static void SetDbWriteLevel<T>(this IDbLoggerSetting loggerSetting, LogLevel logLevel)
        {
            loggerSetting.SetDbWriteLevel(typeof(T).FullName!, logLevel);
        }
    }
    public class LoggerSetting : IFileLoggerSetting, IDbLoggerSetting
    {
        private static readonly Lazy<LoggerSetting> setting = new(() => new());
        public static LoggerSetting Default => setting.Value;
        public string? LogDirectory { get; set; }
        internal event Action? LogLevelSettingChanged;
        internal void NotifyLogLevelSettingChanged()
        {
            LogLevelSettingChanged?.Invoke();
        }
        #region FileLogger 
        public int FileSavedDays { get; set; } = 7;
        public string? LogFileFolder { get; set; }
        public bool SaveByCategory { get; set; }
        public long LogFileSize { get; set; } = 1 * 1024 * 1024;
        internal Func<LogInfo, bool> FileLogInfoFilter { get; set; } = _ => true;

        public void SetFileWriteLevel(string category, LogLevel logLevel)
        {
            var info = LogLimit[LogType.File];
            info.Add(category, logLevel);
        }
        //public void SetCategoryFilter(string category) 
        public void SetFileLogInfoFilter(Func<LogInfo, bool> filter)
        {
            FileLogInfoFilter = filter;
        }
        #endregion

        #region DbLogger
        internal Func<LogInfo, bool> DbLogInfoFilter { get; set; } = _ => true;
        internal Func<IDbLogger>? DbLoggerFacotry { get; set; }

        public void SetDbWriteLevel(string category, LogLevel logLevel)
        {
            var info = LogLimit[LogType.Database];
            info.Add(category, logLevel);
        }
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
        internal class LogLevelInfo : Dictionary<string, LogLevel>
        {
            private readonly LogLevel defaultLevel;
            public LogLevelInfo(LogLevel defaultLevel)
            {
                this.defaultLevel = defaultLevel;
            }

            public LogLevel DefaultLevel => defaultLevel;

            public LogLevel GetLevel(string? category)
            {
                if (string.IsNullOrEmpty(category)) return defaultLevel;
                var key = Keys.FirstOrDefault(k => k == category);
                if (key is null)
                {
                    key = Keys.FirstOrDefault(k => k.StartsWith(category));
                }
                if (key is null)
                {
                    return defaultLevel;
                }
                return this[key];
            }

            public static LogLevelInfo Default(LogLevel level) => new LogLevelInfo(level);
        }
        internal Dictionary<LogType, LogLevelInfo> LogLimit { get; set; } = new()
        {
            [LogType.Debug] = LogLevelInfo.Default(LogLevel.Information),
            [LogType.Console] = LogLevelInfo.Default(LogLevel.Information),
            [LogType.File] = LogLevelInfo.Default(LogLevel.Error),
            [LogType.Database] = LogLevelInfo.Default(LogLevel.Error),
            [LogType.Custom] = LogLevelInfo.Default(LogLevel.None),
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

#if NET6_0_OR_GREATER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    internal class InternalFileLogger : ILogger
    {
        private readonly string category;
        private LogLevel enableLevel;
        private readonly LocalFileLogger fileLogger;
        public InternalFileLogger(string category, IOptions<LoggerSetting> options, LocalFileLogger fileLogger)
        {
            this.category = category;
            this.fileLogger = fileLogger;
            Setting = options.Value;
            enableLevel = Setting.GetLogLevel(LogType.File, category);
        }

        private LoggerSetting Setting { get; set; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= enableLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            if (state?.ToString() == null)
            {
                return;
            }

            var lofInfo = new LogInfo()
            {
                LogLevel = logLevel,
                Message = formatter.Invoke(state, exception),
                EventId = eventId.Id,
                State = state,
                EventName = eventId.Name,
                Category = category,
                Exception = exception
            };

            if (Setting.FileLogInfoFilter(lofInfo))
            {
                fileLogger.WriteLog(lofInfo);
            }
        }
    }
}
#endif
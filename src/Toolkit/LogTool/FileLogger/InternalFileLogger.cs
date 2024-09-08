using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    internal class InternalFileLogger(string category, IOptions<LoggerSetting> options, LocalFileLogger fileLogger) : ILogger
    {
        private readonly string category = category;
        private readonly IOptions<LoggerSetting> options = options;
        private readonly LocalFileLogger fileLogger = fileLogger;

        private LoggerSetting Setting => options.Value;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return Setting.IsEnabled(LogType.File, logLevel);
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

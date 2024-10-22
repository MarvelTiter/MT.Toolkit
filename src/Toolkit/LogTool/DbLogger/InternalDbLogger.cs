#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.DbLogger
{
    internal class InternalDbLogger : ILogger
    {
        private readonly string category;
        private readonly LogLevel enableLevel;
        private readonly IOptions<LoggerSetting> options;
        private readonly DatabaseLogger dbLogger;

        public InternalDbLogger(string category, IOptions<LoggerSetting> options, DatabaseLogger dbLogger)
        {
            this.category = category;
            this.enableLevel = options.Value.GetLogLevel(LogType.Database, category);
            this.options = options;
            this.dbLogger = dbLogger;
        }
        private LoggerSetting Setting => options.Value;
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

            var logInfo = new LogInfo<TState>
            {
                LogLevel = logLevel,
                Message = formatter.Invoke(state, exception),
                State = state,
                EventId = eventId.Id,
                EventName = eventId.Name,
                Category = category,
                Exception = exception
            };
            if (Setting.DbLogInfoFilter(logInfo))
            {
                dbLogger.WriteLog(logInfo);
            }
        }
    }
}
#endif
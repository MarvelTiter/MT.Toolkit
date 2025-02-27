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
        private LogLevel enableLevel;
        private readonly DatabaseLogger dbLogger;
        private readonly LoggerExternalScopeProvider scopeProvider = new LoggerExternalScopeProvider();

        public InternalDbLogger(string category, IOptions<LoggerSetting> options, DatabaseLogger dbLogger)
        {
            this.category = category;
            this.dbLogger = dbLogger;
            Setting = options.Value;
            Setting.LogLevelSettingChanged += Setting_Changed;
            this.enableLevel = Setting.GetLogLevel(LogType.Database, category);
        }

        private void Setting_Changed()
        {
            this.enableLevel = Setting.GetLogLevel(LogType.Database, category);
        }

        private LoggerSetting Setting { get; }
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return scopeProvider.Push(state);
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
            scopeProvider.ForEachScope((scope, list) => list.Add(scope), logInfo.Scopes);
            if (Setting.DbLogInfoFilter(logInfo))
            {
                dbLogger.WriteLog(logInfo);
            }
        }
    }
}
#endif
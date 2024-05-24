using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    internal class InternalFileLogger(string category, IOptionsMonitor<FileLoggerSetting> options) : ILogger
    {
        private FileLoggerSetting Setting => options.CurrentValue;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel >= Setting.WriteLevel;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (typeof(TState) != typeof(string))
            {
                if (Setting.OnlyLogString)
                {
                    return;
                }
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

            if (Setting.CustomCheck(lofInfo))
            {
                LocalFileLogger.Instance.WriteLog(lofInfo);
            }

        }
    }
}

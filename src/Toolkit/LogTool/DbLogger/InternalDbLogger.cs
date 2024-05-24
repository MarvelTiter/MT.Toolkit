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
    internal class InternalDbLogger(string category, IOptionsMonitor<DbLoggerSetting> options, IServiceProvider serviceProvider) : ILogger
    {
        private DbLoggerSetting Setting => options.CurrentValue;
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
            if (Setting.CustomCheck(logInfo))
            {
                var logService = serviceProvider.GetService<IDbLogger<TState>>();
                if (logService == null) return;
                logService.Log(logInfo);
            }
        }
    }
}

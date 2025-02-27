#if NET6_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    internal class FileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, InternalFileLogger> loggers = new();
        private readonly Lazy<LocalFileLogger> fileLogger;
        private readonly IOptions<LoggerSetting> option;
        private readonly IConfiguration configuration;
        private IDisposable reload;
        public FileLoggerProvider(IOptions<LoggerSetting> option, IConfiguration configuration)
        {
            fileLogger = LocalFileLogger.GetFileLogger(option.Value);
            this.option = option;
            this.configuration = configuration;
            Set();
            reload = ChangeToken.OnChange(() => this.configuration.GetReloadToken(), Set);
            
        }
        private void Set()
        {
            var set = configuration.GetSection("Logging:SimpleFileLogger:LogLevel").GetChildren();
            if (!set.Any()) return;
            foreach (var item in set)
            {
                var key = item.Key;
                var value = item.Value;
                if (key is null || value is null) continue;
                if (Enum.TryParse(typeof(LogLevel), value, out var result) && result is LogLevel logLevel)
                {
                    option.Value.AddOrUpdate(LogType.File, key, logLevel);
                }
            }
            option.Value.NotifyLogLevelSettingChanged();
        }
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new InternalFileLogger(categoryName, option, fileLogger.Value));
        }

        public void Dispose()
        {
            loggers.Clear();
            reload?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
#endif
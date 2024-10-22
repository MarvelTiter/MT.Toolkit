#if NET6_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public FileLoggerProvider(IOptions<LoggerSetting> option, IConfiguration configuration)
        {
            fileLogger = LocalFileLogger.GetFileLogger(option.Value);
            this.option = option;
            this.configuration = configuration;
            //var setting = configuration.GetSection("SimpleFileLogger:LogLevel");
            //var reload = setting.GetReloadToken();
            //reload.
            var set = configuration.GetSection("SimpleFileLogger:LogLevel").GetChildren();
            foreach (var item in set)
            {
                var key = item.Key;
                var value = item.Value;
                if (key is null || value is null) continue;
                var logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), value);
                option.Value.AddOrIgnore(LogType.File, key, logLevel);
            }
        }
        public ILogger CreateLogger(string categoryName)
        {
            Console.WriteLine(categoryName);
            return loggers.GetOrAdd(categoryName, new InternalFileLogger(categoryName, option, fileLogger.Value));
        }

        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
#endif
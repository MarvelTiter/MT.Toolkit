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
    internal class FileLoggerProvider(IOptions<LoggerSetting> option) : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, InternalFileLogger> loggers = new();
        private readonly LocalFileLogger fileLogger = LocalFileLogger.GetFileLogger(option.Value).Value;
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new InternalFileLogger(categoryName, option, fileLogger));
        }

        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

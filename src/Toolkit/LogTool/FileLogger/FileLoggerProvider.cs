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
    internal class FileLoggerProvider(IOptionsMonitor<FileLoggerSetting> option) : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, InternalFileLogger> loggers = new();
        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new InternalFileLogger(categoryName, option));
        }

        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.DbLogger;

internal class DbLoggerProvider(IOptionsMonitor<DbLoggerSetting> option, IServiceProvider serviceProvider) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InternalDbLogger> loggers = new();
    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, new InternalDbLogger(categoryName, option,serviceProvider));
    }

    public void Dispose()
    {
        loggers.Clear();
        GC.SuppressFinalize(this);
    }
}

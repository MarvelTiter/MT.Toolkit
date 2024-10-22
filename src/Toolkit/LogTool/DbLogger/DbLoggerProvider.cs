using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.DbLogger;

internal class DbLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InternalDbLogger> loggers = new();
    private readonly DatabaseLogger dbLogger;
    private readonly IOptions<LoggerSetting> option;
    private readonly IServiceProvider serviceProvider;

    public DbLoggerProvider(IOptions<LoggerSetting> option, IServiceProvider serviceProvider)
    {
        this.option = option;
        this.serviceProvider = serviceProvider;
        if (option.Value.DbLoggerFacotry == null)
        {
            option.Value.DbLoggerFacotry = GetDbLogger;
        }
        dbLogger = DatabaseLogger.GetDbLogger(option.Value).Value;
    }

    IDbLogger GetDbLogger()
    {
        var obj = serviceProvider.GetService<IDbLogger>() ?? throw new ArgumentNullException();
        return obj;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, new InternalDbLogger(categoryName, option, dbLogger));
    }

    public void Dispose()
    {
        loggers.Clear();
        GC.SuppressFinalize(this);
    }
}

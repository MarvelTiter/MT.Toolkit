using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.DbLogger;

internal class DbLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, InternalDbLogger> loggers = new();
    private readonly Lazy<DatabaseLogger> dbLogger;
    private readonly IConfiguration configuration;
    private readonly IOptions<LoggerSetting> option;
    private readonly IServiceProvider serviceProvider;
    private IDisposable reload;

    public DbLoggerProvider(IOptions<LoggerSetting> option, IServiceProvider serviceProvider, IConfiguration configuration)
    {
        this.option = option;
        this.serviceProvider = serviceProvider;
        if (option.Value.DbLoggerFacotry == null)
        {
            option.Value.DbLoggerFacotry = GetDbLogger;
        }
        dbLogger = DatabaseLogger.GetDbLogger(option.Value);
        this.configuration = configuration;
        Set();
        reload = ChangeToken.OnChange(() => this.configuration.GetReloadToken(), Set);
    }
    private void Set()
    {
        var set = configuration.GetSection("Logging:SimpleDatabaseLogger").GetChildren();
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
    IDbLogger GetDbLogger()
    {
        var obj = serviceProvider.GetService<IDbLogger>() ?? throw new ArgumentNullException();
        return obj;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, new InternalDbLogger(categoryName, option, dbLogger.Value));
    }

    public void Dispose()
    {
        loggers.Clear();
        reload?.Dispose();
        GC.SuppressFinalize(this);
    }
}
using LoggerProviderExtensions.FileLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace LoggerProviderExtensions.HubLogger;

[ProviderAlias(ALIAS_NAME)]
internal class HubLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    public const string FULL_NAME = "LoggerProviderExtensions.HubLogger.HubLoggerProvider";
    public const string ALIAS_NAME = "Hub";
    private readonly ConcurrentDictionary<string, InternalHubLogger> loggers = new();
    private IExternalScopeProvider scopeProvider = NullScopeProvider.Instance;
    private readonly Lazy<HubLoggerProcesser> hubLogger;
    private readonly IOptionsMonitor<HubLoggerOptions> option;
    public HubLoggerProcesser PL => hubLogger.Value;
    public HubLoggerProvider(IOptionsMonitor<HubLoggerOptions> option)
    {
        hubLogger = HubLoggerProcesser.GetHubLogger(option.CurrentValue);
        this.option = option;
    }

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, c =>
        {
            return new InternalHubLogger(categoryName, hubLogger.Value, option.CurrentValue, scopeProvider);
        });
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public void SetScopeProvider(IExternalScopeProvider scopeProvider)
    {
        this.scopeProvider = scopeProvider;
        foreach (var item in loggers)
        {
            item.Value.ScopeProvider = scopeProvider;
        }
    }
}

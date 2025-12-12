#if NET6_0_OR_GREATER
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.FileLogger;

[UnsupportedOSPlatform("browser")]
[ProviderAlias(ALIAS_NAME)]
internal class FileLoggerProvider : ILoggerProvider, ISupportExternalScope
{
    public const string FULL_NAME = "LoggerProviderExtensions.FileLogger.FileLoggerProvider";
    public const string ALIAS_NAME = "LocalFile";
    private readonly ConcurrentDictionary<string, InternalFileLogger> loggers = new();
    private readonly Lazy<LocalFileLoggerProcessor> fileLogger;
    private readonly IOptionsMonitor<FileLoggerOptions> option;
    private readonly IOptionsMonitor<LoggerFilterOptions> filters;
    private readonly IDisposable? reload;
    private readonly IDisposable? reloadFilters;
    private IExternalScopeProvider scopeProvider = NullScopeProvider.Instance;
    public FileLoggerProvider(IOptionsMonitor<FileLoggerOptions> option, IOptionsMonitor<LoggerFilterOptions> filters)
    {
        fileLogger = LocalFileLoggerProcessor.GetFileLogger(option.CurrentValue);
        this.option = option;
        this.filters = filters;
        ReloadOptions(this.option.CurrentValue);
        reload = this.option.OnChange(ReloadOptions);
        ReApplyFilters(this.filters.CurrentValue);
        reloadFilters = this.filters.OnChange(ReApplyFilters);
    }
    private void ReloadOptions(FileLoggerOptions options)
    {
        fileLogger.Value.LogConfig = options;
        foreach (var item in loggers)
        {
            item.Value.Setting = options;
        }
    }
    private void ReApplyFilters(LoggerFilterOptions options)
    {
        foreach (var item in loggers)
        {
            if (options.TryGetOverriddenSettingFilter(FULL_NAME, ALIAS_NAME, item.Key, out var rule, out var isProviderScope))
            {
                var level = rule.LogLevel;
                if (level.HasValue)
                {
                    if (isProviderScope || level.Value >= option.CurrentValue.MinLevel)
                    {
                        item.Value.MinLevel = level.Value;
                    }
                }
            }
        }
    }
    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(categoryName, c =>
        {
            filters.CurrentValue.TryGetOverriddenSettingFilter(FULL_NAME, ALIAS_NAME, c, out var rule, out var isProviderScope);
            var logLevel = rule?.LogLevel;
            var fallback = option.CurrentValue.MinLevel;
            // LocalFile下设置的LogLevel优先级最高，直接使用
            // 否则，全局设置的LogLevel需要比LocalFile的MinLevel更高才能生效
            LogLevel level = logLevel.HasValue
                ? (isProviderScope ? logLevel.Value : (logLevel.Value > fallback ? logLevel.Value : fallback))
                : fallback;
            return new InternalFileLogger(c, option.CurrentValue, fileLogger.Value, scopeProvider, level);
        });
    }

    public void Dispose()
    {
        loggers.Clear();
        reload?.Dispose();
        reloadFilters?.Dispose();
        GC.SuppressFinalize(this);
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
#endif
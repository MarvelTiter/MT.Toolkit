using Microsoft.Extensions.Logging;

namespace LoggerProviderExtensions.HubLogger;

internal class InternalHubLogger(string category
        , HubLoggerProcesser logger
        , HubLoggerOptions setting
        , IExternalScopeProvider scopeProvider
    ) : ILogger
{
    public HubLoggerOptions Setting { get; } = setting;
    public IExternalScopeProvider ScopeProvider { get; set; } = scopeProvider;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return ScopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var structuredLog = new StructuredLogEntry
        {
            Timestamp = Setting.UseUtcTimestamp
               ? DateTimeOffset.UtcNow
               : DateTimeOffset.Now,
            LogLevel = logLevel,
            Category = category,
            EventId = eventId.Id,
            EventName = eventId.Name,
            // 提取消息模板（原始格式串，如 "用户 {UserId} 登录"）
            MessageTemplate = state?.ToString(),

            // 提取结构化属性
            Properties = Formatter.ExtractProperties(state),
            // 提取 Scope 数据
            Scopes = Formatter.ExtractScopes(Setting.IncludeScopes, ScopeProvider),
            Exception = exception
        };
        logger.WriteLog(structuredLog);
    }
}

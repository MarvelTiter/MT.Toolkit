using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace LoggerProviderExtensions.DbLogger;

internal class InternalDbLogger(string category
    , DbLoggerOptions options
    , DatabaseLoggerProcessor dbLogger
    , IExternalScopeProvider scopeProvider
    , LogLevel minLevel) : ILogger
{

    public DbLoggerOptions Setting { get; set; } = options;
    public IExternalScopeProvider ScopeProvider { get; set; } = scopeProvider;
    public LogLevel MinLevel { get; set; } = minLevel;
    [ThreadStatic]
    private static StringWriter? t_stringWriter;
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return ScopeProvider.Push(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= MinLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }
        t_stringWriter ??= new();
        var sb = t_stringWriter.GetStringBuilder();
        if (Setting.Structured)
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
            var sf = Setting.StructuredFormatter ?? SerilogCompactJsonFormatter.Default.Value;
            sf.Format(t_stringWriter, structuredLog);
        }
        else
        {
            LogEntry<TState> logEntry = new(logLevel, category, eventId, state, exception, formatter);
            Formatter.FormatDbContent(logEntry, ScopeProvider, t_stringWriter, Setting);
        }
        if (sb.Length == 0)
        {
            return;
        }
        string message = sb.ToString();
        sb.Clear();

        dbLogger.WriteLog(new(message, exception is not null, Formatter.GetCurrentDateTime(Setting)));
    }
}
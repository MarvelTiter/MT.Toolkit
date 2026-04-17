#if NET6_0_OR_GREATER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Reflection;

namespace LoggerProviderExtensions.FileLogger;

internal class InternalFileLogger(string category
        , FileLoggerOptions options
        , LocalFileLoggerProcessor fileLogger
        , IExternalScopeProvider scopeProvider
        , LogLevel logLevel) : ILogger
{
    public IExternalScopeProvider ScopeProvider { get; set; } = scopeProvider;
    public FileLoggerOptions Setting { get; set; } = options;
    public LogLevel MinLevel { get; set; } = logLevel;
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
            Formatter.FormatFileContent(logEntry, ScopeProvider, t_stringWriter, Setting);
        }
        if (sb.Length == 0)
        {
            return;
        }
        string message = sb.ToString();
        sb.Clear();

        fileLogger.WriteLog(category, message, logLevel);
    }

}
#endif
#if NET6_0_OR_GREATER
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
        LogEntry<TState> logEntry = new(logLevel, category, eventId, state, exception, formatter);
        t_stringWriter ??= new();
        Formatter.FormatFileContent(logEntry, ScopeProvider, t_stringWriter, Setting);
        var sb = t_stringWriter.GetStringBuilder();
        if (sb.Length == 0)
        {
            return;
        }
        string message = sb.ToString();
        sb.Clear();
        //var logInfo = new LogInfo()
        //{
        //    LogLevel = logLevel,
        //    Message = formatter.Invoke(state, exception),
        //    EventId = eventId.Id,
        //    State = state,
        //    EventName = eventId.Name,
        //    Category = category,
        //    Exception = exception
        //};

        //ScopeProvider.ForEachScope(static (scope, list) => list.Add(scope), logInfo.Scopes);

        fileLogger.WriteLog(category, message);
    }

}
#endif
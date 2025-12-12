using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        LogEntry<TState> logEntry = new(logLevel, category, eventId, state, exception, formatter);
        t_stringWriter ??= new();
        Formatter.FormatDbContent(logEntry, ScopeProvider, t_stringWriter, Setting);
        var sb = t_stringWriter.GetStringBuilder();
        if (sb.Length == 0)
        {
            return;
        }
        string message = sb.ToString();
        sb.Clear();
        //var logInfo = new LogInfo<TState>
        //{
        //    LogLevel = logLevel,
        //    Message = formatter.Invoke(state, exception),
        //    State = state,
        //    EventId = eventId.Id,
        //    EventName = eventId.Name,
        //    Category = category,
        //    Exception = exception
        //};

        dbLogger.WriteLog(new(message, exception is not null, Formatter.GetCurrentDateTime(Setting)));
    }
}
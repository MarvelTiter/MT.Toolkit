using LoggerProviderExtensions.DbLogger;
using LoggerProviderExtensions.FileLogger;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.IO;
using static System.Formats.Asn1.AsnWriter;

namespace LoggerProviderExtensions;

internal class Formatter
{
    private static readonly string messagePadding = new(' ', 4);
    private static readonly string newLineWithMessagePadding = Environment.NewLine + messagePadding;
    public static DateTimeOffset GetCurrentDateTime<TOptions>(TOptions options)
        where TOptions : BaseLoggerOptions
    {
        return options.TimestampFormat != null
            ? (options.UseUtcTimestamp ? DateTimeOffset.UtcNow : DateTimeOffset.Now)
            : DateTimeOffset.MinValue;
    }
    public static void FormatFileContent<TState>(in LogEntry<TState> entry, IExternalScopeProvider scope, StringWriter textWriter, FileLoggerOptions options)
    {
        var message = entry.Formatter(entry.State, entry.Exception);
        var logLevelString = entry.LogLevel.GetLogLevelString();
        var stamp = GetCurrentDateTime(options);
        string timestamp;
        string? timestampFormat = options.TimestampFormat;
        if (timestampFormat != null)
        {
            timestamp = stamp.ToString(timestampFormat);
        }
        else
        {
            timestamp = stamp.ToString();
        }
        WriteFile(scope, textWriter, logLevelString, entry.Category, entry.EventId.Id, message, timestamp, entry.Exception?.ToString(), options.IncludeScopes, false);
    }

    public static void FormatDbContent<TState>(in LogEntry<TState> entry, IExternalScopeProvider scope, StringWriter textWriter, DbLoggerOptions options)
    {
        var message = entry.Formatter(entry.State, entry.Exception);
        var logLevelString = entry.LogLevel.GetLogLevelString();
        WriteFile(scope, textWriter, logLevelString, entry.Category, entry.EventId.Id, message, string.Empty, entry.Exception?.ToString(), options.IncludeScopes, true);
    }

    private static void WriteFile(IExternalScopeProvider scope, StringWriter textWriter
        , string? logLevelString
        , string category
        , int eventId
        , string message
        , string timestamp
        , string? exception
        , bool includeScoped
        , bool singleLine)
    {
        textWriter.Write('[');
        textWriter.Write(timestamp);
        textWriter.Write(']');

        if (logLevelString != null)
        {
            textWriter.Write('[');
            textWriter.Write(logLevelString);
            textWriter.Write(']');
        }

        textWriter.Write(':');
        textWriter.Write(category);
        textWriter.Write('[');

#if NET
        Span<char> span = stackalloc char[10];
        if (eventId.TryFormat(span, out int charsWritten))
            textWriter.Write(span.Slice(0, charsWritten));
        else
#endif
            textWriter.Write(eventId.ToString());

        textWriter.Write(']');
        if (!singleLine)
            textWriter.Write(Environment.NewLine);

        // scope information
        WriteScopeInformation(textWriter, scope, includeScoped, singleLine);
        WriteMessage(textWriter, message, false);

        // Example:
        // System.InvalidOperationException
        //    at Namespace.Class.Function() in File:line X
        if (exception != null)
        {
            // exception message
            WriteMessage(textWriter, exception, singleLine);
        }
    }

    private static void WriteScopeInformation(TextWriter textWriter, IExternalScopeProvider? scopeProvider, bool includeScopeds, bool singleLine)
    {
        if (includeScopeds && scopeProvider != null)
        {
            bool paddingNeeded = !singleLine;
            scopeProvider.ForEachScope((scope, state) =>
            {
                if (paddingNeeded)
                {
                    paddingNeeded = false;
                    state.Write(messagePadding);
                    state.Write("=> ");
                }
                else
                {
                    state.Write(" => ");
                }
                state.Write(scope);
            }, textWriter);

            if (!paddingNeeded && !singleLine)
            {
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    private static void WriteMessage(TextWriter textWriter, string message, bool singleLine)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (singleLine)
            {
                textWriter.Write(' ');
                WriteReplacing(textWriter, Environment.NewLine, " ", message);
            }
            else
            {
                textWriter.Write(messagePadding);
                WriteReplacing(textWriter, Environment.NewLine, newLineWithMessagePadding, message);
                textWriter.Write(Environment.NewLine);
            }
        }

        static void WriteReplacing(TextWriter writer, string oldValue, string newValue, string message)
        {
            string newMessage = message.Replace(oldValue, newValue);
            writer.Write(newMessage);
        }
    }

    public static Dictionary<string, object?> ExtractProperties<TState>(TState state)
    {
        var properties = new Dictionary<string, object?>();

        if (state == null) return properties;

        if (state is IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            foreach (var kv in keyValuePairs)
            {
                // {OriginalFormat} 是模板字符串本身，通常不需要存入 Properties
                if (kv.Key == "{OriginalFormat}") continue;

                properties[kv.Key] = kv.Value;
            }
        }

        return properties;
    }
    public static List<Dictionary<string, object?>>? ExtractScopes(bool includeScopeds, IExternalScopeProvider? scopeProvider)
    {
        if (includeScopeds && scopeProvider != null)
        {
            var scopes = new List<Dictionary<string, object?>>();
            scopeProvider?.ForEachScope((scope, state) =>
            {
                var scopeDict = new Dictionary<string, object?>();

                if (scope is IEnumerable<KeyValuePair<string, object>> pairs)
                {
                    foreach (var kv in pairs)
                    {
                        if (kv.Key == "{OriginalFormat}") continue;
                        scopeDict[kv.Key] = kv.Value;
                    }
                }
                else if (scope != null)
                {
                    // 如果是字符串或其他简单类型
                    scopeDict["Value"] = scope;
                }

                scopes.Add(scopeDict);

            }, scopes);
            return scopes;
        }
        return null;
    }
}

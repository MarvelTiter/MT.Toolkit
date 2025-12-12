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
    private static readonly string messagePadding = new string(' ', 4);
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
        //if (singleLine)
        textWriter.Write(Environment.NewLine);
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
}

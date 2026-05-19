using System.Text.Json;
using System.Text.Json.Serialization;

namespace LoggerProviderExtensions;

/// <summary>
/// Serilog Compact Json Formatter
/// </summary>
public class SerilogCompactJsonFormatter : IStructuredLogger
{
    internal static readonly Lazy<SerilogCompactJsonFormatter> Default = new(() => new SerilogCompactJsonFormatter());
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Format
    /// </summary>
    /// <param name="stringWriter"></param>
    /// <param name="entry"></param>
    public void Format(StringWriter stringWriter, StructuredLogEntry entry)
    {
        var output = new Dictionary<string, object?>
        {
            ["@t"] = entry.Timestamp.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"),
            ["@l"] = entry.LogLevel.ToString(),
            ["@mt"] = entry.MessageTemplate,
            ["@c"] = entry.Category,
            ["@i"] = entry.EventId
        };

        if (entry.Properties != null)
        {
            foreach (var kv in entry.Properties)
            {
                output[kv.Key] = kv.Value;
            }
        }
        if (entry.Scopes != null && entry.Scopes.Count > 0)
        {
            var scopeArray = new List<Dictionary<string, object?>>();

            foreach (var scope in entry.Scopes)
            {
                var scopeDict = new Dictionary<string, object?>();

                foreach (var kv in scope)
                {
                    // 展开到根对象（业务查询最常用）
                    var key = kv.Key.StartsWith('@') ? "_" + kv.Key : kv.Key;

                    // 如果多个 Scope 有同名键，后面的会覆盖前面的
                    // 这是 Serilog 的行为，符合大多数场景
                    output[key] = kv.Value;

                    // 同时保留在 Scope 层级结构中
                    scopeDict[kv.Key] = kv.Value;
                }

                if (scopeDict.Count > 0)
                {
                    scopeArray.Add(scopeDict);
                }
            }

            if (scopeArray.Count > 0)
            {
                output["@s"] = scopeArray;
            }
        }
        if (entry.Exception != null)
        {
            output["@x"] = entry.Exception.Message;
            output["@ex"] = entry.Exception.StackTrace;
        }

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
#pragma warning disable IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
        stringWriter.Write(JsonSerializer.Serialize(output, _options));
#pragma warning restore IL3050 // Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
    }
}

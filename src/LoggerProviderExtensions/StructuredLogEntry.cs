using Microsoft.Extensions.Logging;

namespace LoggerProviderExtensions;

/// <summary>
/// 结构化日志上下文
/// </summary>
public readonly struct StructuredLogEntry
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
    /// <summary>
    /// 日志级别
    /// </summary>
    public LogLevel LogLevel { get; init; }
    /// <summary>
    /// 日志类别
    /// </summary>
    public string Category { get; init; }
    /// <summary>
    /// 事件ID
    /// </summary>
    public int EventId { get; init; }
    /// <summary>
    /// 事件名称
    /// </summary>
    public string? EventName { get; init; }
    /// <summary>
    /// 日志消息模板
    /// </summary>
    public string? MessageTemplate { get; init; }
    /// <summary>
    /// 日志消息参数
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Properties { get; init; }
    /// <summary>
    /// 作用域信息
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, object?>>? Scopes { get; init; }
    /// <summary>
    /// 异常
    /// </summary>
    public Exception? Exception { get; init; }
}

using Microsoft.Extensions.Configuration;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace LoggerProviderExtensions;

/// <summary>
/// 
/// </summary>
public class BaseLoggerOptions
{
    /// <summary>
    /// 是否包含作用域信息
    /// </summary>
    public bool IncludeScopes { get; set; }
    /// <summary>
    /// 时间格式化字符串
    /// </summary>
    [StringSyntax(StringSyntaxAttribute.DateTimeFormat)]
    public string? TimestampFormat { get; set; }
    /// <summary>
    /// 是否启用UTC时间
    /// </summary>
    public bool UseUtcTimestamp { get; set; }
    /// <summary>
    /// 是否结构化日志
    /// </summary>
    public bool Structured { get; set; }
    /// <summary>
    /// 是否结构化日志
    /// </summary>
    public IStructuredLogger? StructuredFormatter { get; set; }
}

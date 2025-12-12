using System.Diagnostics.CodeAnalysis;

namespace LoggerProviderExtensions.FileLogger;

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
    
}

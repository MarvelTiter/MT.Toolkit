using System;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace LoggerProviderExtensions;

[Flags]
[Obsolete("不再使用")]
public enum LogType
{
    File = 1,
    Database = 1 << 1,
}
/// <summary>
/// 日志级别
/// </summary>
public enum SimpleLogLevel
{
    /// <summary>
    /// 
    /// </summary>
    Trace,
    /// <summary>
    /// 信息级别
    /// </summary>
    Information,
    /// <summary>
    /// debug级别
    /// </summary>
    Debug,
    /// <summary>
    /// 警告级别
    /// </summary>
    Warning,
    /// <summary>
    /// 错误级别
    /// </summary>
    Error,
    /// <summary>
    /// 致命级别
    /// </summary>
    Critical,
    /// <summary>
    /// 
    /// </summary>
    None,
}

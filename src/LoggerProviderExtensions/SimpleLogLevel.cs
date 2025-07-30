using System;

namespace LoggerProviderExtensions;

[Flags]
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
    None,
}

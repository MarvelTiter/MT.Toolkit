using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.FileLogger;

/// <summary>
/// 本地文件日志配置
/// </summary>
[Obsolete]
public interface IFileLoggerSetting
{
    /// <summary>
    /// 文件保留天数，默认7天
    /// </summary>
    int FileSavedDays { get; set; }
    /// <summary>
    /// 日志保存目录
    /// </summary>
    string? LogFileFolder { get; set; }
    /// <summary>
    /// 文件大小，默认1m
    /// </summary>
    long LogFileSize { get; set; }
    /// <summary>
    /// 按Category分类保存日志文件
    /// </summary>
    public bool SaveByCategory { get; set; }
    /// <summary>
    /// 设置写入文件的日志级别
    /// </summary>
    void SetFileWriteLevel(string category, LogLevel logLevel);
    /// <summary>
    /// 设置写入文件的日志过滤
    /// </summary>
    void SetFileLogInfoFilter(Func<LogInfo, bool> filter);
}

/// <summary>
/// 本地文件日志配置
/// </summary>
public class FileLoggerOptions: BaseLoggerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public FileLoggerOptions()
    {
        TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    }
    /// <summary>
    /// 文件保留天数，默认7天
    /// </summary>
    public int FileSavedDays { get; set; } = 7;
    /// <summary>
    /// 日志保存目录
    /// </summary>
    public string? LogFileFolder { get; set; }
    /// <summary>
    /// 文件大小，默认1m
    /// </summary>
    public long LogFileSize { get; set; } = 1 * 1024 * 1024;
    /// <summary>
    /// 按Category分类保存日志文件
    /// </summary>
    public bool SaveByCategory { get; set; }
    /// <summary>
    /// <para>
    /// 文件日志记录的最小级别, 默认是Error, 低于此级别的日志将不会被记录到文件中
    /// </para>
    /// <para>
    /// 该优先级高于全局的LogLevel设置, 但是低于LocalFile下的LogLevel设置
    /// </para>
    /// </summary>
    public LogLevel MinLevel { get; set; } = LogLevel.Error;
}

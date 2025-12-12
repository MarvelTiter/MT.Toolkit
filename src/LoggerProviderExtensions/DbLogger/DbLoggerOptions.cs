using LoggerProviderExtensions.FileLogger;
using Microsoft.Extensions.Logging;

namespace LoggerProviderExtensions.DbLogger;

/// <summary>
/// 数据库日志配置
/// </summary>
public class DbLoggerOptions : BaseLoggerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public DbLoggerOptions()
    {
        TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    }
    /// <summary>
    /// <para>
    /// 日志记录的最小级别, 默认是Error, 低于此级别的日志将不会被记录到数据库中
    /// </para>
    /// <para>
    /// 该优先级高于全局的LogLevel设置, 但是低于DatabaseLog下的LogLevel设置
    /// </para>
    /// </summary>
    public LogLevel MinLevel { get; set; } = LogLevel.Error;
    /// <summary>
    /// 数据库日志记录器
    /// </summary>
    public Func<IDbLogger>? DbLoggerFacotry { get; set; }
}
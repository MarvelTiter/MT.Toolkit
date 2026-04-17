namespace LoggerProviderExtensions;

/// <summary>
/// 结构化日志格式化接口
/// </summary>
public interface IStructuredLogger
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="stringWriter"></param>
    /// <param name="entry"></param>
    void Format(StringWriter stringWriter, StructuredLogEntry entry);
}

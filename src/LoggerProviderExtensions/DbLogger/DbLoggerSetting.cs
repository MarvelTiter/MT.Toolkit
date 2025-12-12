using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace LoggerProviderExtensions.DbLogger;

[Obsolete("不再使用")]
public interface IDbLoggerSetting
{
    void SetDbWriteLevel(string category, LogLevel logLevel);
    void SetDbLogInfoFilter(Func<LogInfo, bool> filter);
    /// <summary>
    /// 指定如何获取IDbLogger实例，否则默认从Ioc容器中获取该实例
    /// </summary>
    void SetDbLogger(Func<IDbLogger> factory);
}

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.DbLogger;

public interface IDbLoggerSetting
{
    void SetDbWriteLevel(string category, LogLevel logLevel);
    void SetDbLogInfoFilter(Func<LogInfo, bool> filter);
    /// <summary>
    /// 指定如何获取IDbLogger实例，否则默认从Ioc容器中获取该实例
    /// </summary>
    void SetDbLogger(Func<IDbLogger> factory);
}

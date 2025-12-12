using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.DbLogger;

/// <summary>
/// 数据库日志记录接口
/// </summary>
public interface IDbLogger
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task LogAsync(DbLogItem item, CancellationToken cancellationToken);
}

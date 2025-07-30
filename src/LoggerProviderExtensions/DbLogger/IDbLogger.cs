using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.DbLogger;

public interface IDbLogger
{
    Task LogAsync(LogInfo data, CancellationToken cancellationToken);
}

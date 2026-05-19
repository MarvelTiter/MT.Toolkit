using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.HubLogger;

/// <summary>
/// 
/// </summary>
public interface IHubLoggerPublisher
{
    /// <summary>
    /// 
    /// </summary>
    event Action<StructuredLogEntry>? OnLog;
    ///// <summary>
    ///// 
    ///// </summary>
    //IAsyncEnumerable<StructuredLogEntry> LogEntries { get; }
}

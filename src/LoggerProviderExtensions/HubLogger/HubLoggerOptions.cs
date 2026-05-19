using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerProviderExtensions.HubLogger;

/// <summary>
/// 
/// </summary>
public class HubLoggerOptions : BaseLoggerOptions
{
    /// <summary>
    /// 
    /// </summary>
    public int Capacity { get; set; } = 2000;
}

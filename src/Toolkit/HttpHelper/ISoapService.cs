using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;
using System.Threading;
namespace MT.Toolkit.HttpHelper
{
    /// <summary>
    /// 
    /// </summary>
    [Obsolete("Use SoapRequestHelper Package instead.", true)]
    public interface ISoapService : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);
    }
}

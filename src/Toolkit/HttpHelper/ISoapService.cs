using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;
using System.Threading;
namespace MT.Toolkit.HttpHelper
{

    public interface ISoapService : IDisposable
    {
        Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);

        Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);
    }
}

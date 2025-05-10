using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using System.Net.Http;
namespace MT.Toolkit.HttpHelper
{

    public interface ISoapService //: IDisposable
    {
        void SetHttpClient(HttpClient client);
        Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null);

        Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
namespace MT.Toolkit.HttpHelper
{

    public interface ISoapService //: IDisposable
    {
        Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object> args);
    }
}
#endif
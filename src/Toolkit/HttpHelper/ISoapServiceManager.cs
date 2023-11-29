


#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
using System;
using System.Collections.Concurrent;
namespace MT.Toolkit.HttpHelper
{
    public interface ISoapServiceManager
    {
        internal ConcurrentDictionary<string, SoapServiceConfiguration> Configs { get; }
        internal string? DefaultKey {  get; }
        ISoapServiceManager SetDefault(string key);
        ISoapServiceManager AddSoapService(string key, Action<SoapServiceConfiguration> config);
    }
}
#endif
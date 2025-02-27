using System;
using System.Collections.Concurrent;
namespace MT.Toolkit.HttpHelper
{
    public interface ISoapServiceManager
    {
        internal ConcurrentDictionary<string, SoapServiceConfiguration> Configs { get; }
        internal string? DefaultKey {  get; }
        ISoapServiceManager SetDefault(string key);
        ISoapServiceManager AddSoapService(string name, Action<SoapServiceConfiguration> config);
    }
}
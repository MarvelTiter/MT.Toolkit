using System;
using System.Collections.Concurrent;


#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
namespace MT.Toolkit.HttpHelper
{
    public class SoapServiceManager : ISoapServiceManager
    {
        private readonly ConcurrentDictionary<string, SoapServiceConfiguration> configs = new();
        private string? defaultKey;
        public ConcurrentDictionary<string, SoapServiceConfiguration> Configs => configs;
        public string? DefaultKey => defaultKey;

        public ISoapServiceManager AddSoapService(string name, Action<SoapServiceConfiguration> config)
        {
            if (!configs.ContainsKey(name))
            {
                var c = new SoapServiceConfiguration(name);
                config.Invoke(c);
                if (string.IsNullOrEmpty(c.Url))
                {
                    throw new ArgumentNullException("AddSoapService Url is null");
                }
                configs[name] = c;
            }
            return this;
        }

        public ISoapServiceManager SetDefault(string key)
        {
            defaultKey = key;
            return this;
        }
    }
}
#endif
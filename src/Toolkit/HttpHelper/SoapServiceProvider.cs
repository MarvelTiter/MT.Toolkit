using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Concurrent;

namespace MT.Toolkit.HttpHelper
{
    public class SoapServiceProvider : ISoapServiceFactory
    {
        private readonly IServiceProvider provider;
        private readonly ISoapServiceManager soapServiceManager;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ConcurrentDictionary<string, SoapService> services = [];
        public SoapServiceProvider(IServiceProvider provider, ISoapServiceManager soapServiceManager, IHttpClientFactory httpClientFactory)
        {
            this.provider = provider;
            this.soapServiceManager = soapServiceManager;
            this.httpClientFactory = httpClientFactory;
        }
        public ISoapService? Default
        {
            get
            {
                if (soapServiceManager.DefaultKey != null)
                    return GetSoapService(soapServiceManager.DefaultKey);
                return null;
            }
        }

        public ISoapService GetSoapService(string key)
        {
            return services.GetOrAdd(key, (name) =>
             {
                 if (soapServiceManager.Configs.TryGetValue(name, out var config))
                 {
                     return new SoapService(httpClientFactory, config);
                 }
                 throw new ArgumentNullException($"未注册SoapService[{name}]");
             });
        }
    }
}
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace MT.Toolkit.HttpHelper
{
    public class SoapServiceProvider : ISoapServiceFactory
    {
        private readonly IServiceProvider provider;
        private readonly ISoapServiceManager soapServiceManager;
        private readonly IHttpClientFactory httpClientFactory;

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
            var config = soapServiceManager.Configs[key];
            //Func<string, HttpClient> factory;
            //if (config.ClientProvider != null)
            //{
            //    factory = s => config.ClientProvider.Invoke(new ProviderContext(httpClientFactory, s));
            //}
            //else
            //{
            //    factory = s => httpClientFactory.CreateClient();
            //}

            return new SoapService(httpClientFactory, config, provider.GetService<ILogger<SoapService>>()!, provider);
        }
    }
}
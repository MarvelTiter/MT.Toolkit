using System;


#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

namespace MT.Toolkit.HttpHelper
{
    public class SoapServiceProvider : ISoapServiceFactory
    {
        private readonly IServiceProvider provider;
        private readonly ISoapServiceManager soapServiceManager;

        public SoapServiceProvider(IServiceProvider provider, ISoapServiceManager soapServiceManager)
        {
            this.provider = provider;
            this.soapServiceManager = soapServiceManager;
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
            var client = config.ClientProvider?.Invoke(provider) ?? provider.GetService<IHttpClientFactory>()?.CreateClient();
            if (client == null)
                throw new Exception("can not get httpclient instance");
            return new SoapService(client, config);
        }
    }
}
#endif
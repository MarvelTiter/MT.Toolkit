using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Concurrent;

namespace MT.Toolkit.HttpHelper
{
    internal class SoapServiceProvider : ISoapServiceFactory
    {
        private readonly IServiceProvider provider;
        private readonly ISoapServiceManager soapServiceManager;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ConcurrentDictionary<string, SoapService> services = [];
        private bool disposedValue;
        private ILogger logger;
        public SoapServiceProvider(IServiceProvider provider
            , ISoapServiceManager soapServiceManager
            , IHttpClientFactory httpClientFactory
            , ILogger<ISoapServiceFactory> logger)
        {
            this.provider = provider;
            this.soapServiceManager = soapServiceManager;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
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

        internal void Log(string message)
        {
            logger.LogInformation("{message}", message);
        }

        public ISoapService GetSoapService(string key)
        {
            return services.GetOrAdd(key, (name) =>
             {
                 if (soapServiceManager.Configs.TryGetValue(name, out var config))
                 {
                     return new SoapService(httpClientFactory, config, Log);
                 }
                 throw new ArgumentNullException($"未注册SoapService[{name}]");
             });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    foreach (var item in services.Values)
                    {
                        item.Dispose();
                    }
                }

                disposedValue = true;
            }
        }

        ~SoapServiceProvider()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
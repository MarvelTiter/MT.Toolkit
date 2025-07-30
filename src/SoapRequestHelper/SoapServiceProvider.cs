using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Concurrent;

namespace SoapRequestHelper;

internal class SoapServiceProvider : ISoapServiceFactory
{
    private readonly ISoapServiceManager soapServiceManager;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger logger;
    private readonly ConcurrentDictionary<string, SoapService> services = [];
    private bool disposedValue;
    public SoapServiceProvider(ISoapServiceManager soapServiceManager
        , IHttpClientFactory httpClientFactory
        , ILogger<ISoapServiceFactory> logger)
    {
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

    public async ValueTask DisposeAsync()
    {
        if (disposedValue) return;
        foreach (var item in services.Values)
        {
            await item.DisposeAsync();
        }
        disposedValue = true;
    }

}
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Collections.Concurrent;

namespace SoapRequestHelper;

internal class SoapServiceProvider : ISoapServiceFactory
{
    private readonly ISoapServiceManager soapServiceManager;
    private readonly ILogger logger;
    private readonly ConcurrentDictionary<string, SoapService> services = [];
    private bool disposedValue;
    public SoapServiceProvider(ISoapServiceManager soapServiceManager
        , ILogger<ISoapServiceFactory> logger)
    {
        this.soapServiceManager = soapServiceManager;
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
                 return new SoapService(config, Log);
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
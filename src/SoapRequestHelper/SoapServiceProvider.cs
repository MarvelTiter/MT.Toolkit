using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace SoapRequestHelper;

internal class SoapServiceProvider(ISoapServiceManager soapServiceManager
        , ILogger<ISoapServiceFactory> logger
        , IHttpClientFactory clientFactory
        , IServiceProvider serviceProvider) : ISoapServiceFactory
{
    private readonly ILogger logger = logger;
    private readonly ConcurrentDictionary<string, SoapService> services = [];
    private bool disposedValue;

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
                 if (config.ClientPoolFactory is not null)
                 {
                     config.UseHttpClientPool(config.ClientPoolFactory(serviceProvider));
                 }
                 else if (config.HttpClientPool is null)
                 {
                     config.UseHttpClientPool(new EmptyPool(clientFactory));
                 }
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
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SoapRequestHelper;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// <para>
    /// 内部通过<see cref="Microsoft.Extensions.Logging.ILogger"/>记录日志
    /// </para>
    /// <para>
    /// 泛型参数<see cref="ISoapServiceFactory"/>
    /// </para>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="manager"></param>
    /// <returns></returns>
    public static IServiceCollection AddSoapServiceHelper(this IServiceCollection services, Action<ISoapServiceManager> manager)
    {
        var serviceManager = new SoapServiceManager();
        manager.Invoke(serviceManager);
        services.AddSingleton<ISoapServiceManager>(serviceManager);
        services.AddSingleton<ISoapServiceFactory, SoapServiceProvider>();
        return services;
    }
}

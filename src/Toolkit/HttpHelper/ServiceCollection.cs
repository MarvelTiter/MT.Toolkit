using System;
#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using Microsoft.Extensions.DependencyInjection;

namespace MT.Toolkit.HttpHelper
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSoapServiceHelper(this IServiceCollection services, Action<ISoapServiceManager> manager)
        {
            var serviceManager = new SoapServiceManager();
            manager.Invoke(serviceManager);
            services.AddSingleton<ISoapServiceManager>(serviceManager);
            services.AddSingleton<ISoapServiceFactory, SoapServiceProvider>();
            return services;
        }

    }
}
#endif

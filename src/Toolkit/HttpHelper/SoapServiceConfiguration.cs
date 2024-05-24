
using System;
using System.Net.Http;

namespace MT.Toolkit.HttpHelper
{
    public class ProviderContext
    {
        public ProviderContext(IHttpClientFactory factory,IServiceProvider? services, string methodName)
        {
            Factory = factory;
            Services = services;
            MethodName = methodName;
        }
        public IServiceProvider? Services { get; }
        public IHttpClientFactory Factory { get; }
        public string MethodName { get; }
    }
    public class SoapServiceConfiguration
    {
        public Func<ProviderContext, HttpClient>? ClientProvider { get; set; }
        public string? Url { get; set; }
        public SoapVersion? Version { get; set; }
        public string? RequestNamespace { get; set; }
        public string? ResponseNamespace { get; set; }
    }
}


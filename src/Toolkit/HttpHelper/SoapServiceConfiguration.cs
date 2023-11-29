#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER

using System;
using System.Net.Http;

namespace MT.Toolkit.HttpHelper
{
    public class SoapServiceConfiguration
    {
        public Func<IServiceProvider, HttpClient>? ClientProvider { get; set; }
        public string? Url { get; set; }
        public SoapVersion? Version { get; set; }
        public string? RequestNamespace { get; set; }
        public string? ResponseNamespace { get; set; }
    }
}
#endif

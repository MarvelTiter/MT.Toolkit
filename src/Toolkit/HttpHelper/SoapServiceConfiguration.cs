
using System;
using System.Net.Http;

namespace MT.Toolkit.HttpHelper
{
    //public class ProviderContext
    //{
    //    public ProviderContext(IHttpClientFactory factory,IServiceProvider? services, string methodName)
    //    {
    //        Factory = factory;
    //        Services = services;
    //        MethodName = methodName;
    //    }
    //    public IServiceProvider? Services { get; }
    //    public IHttpClientFactory Factory { get; }
    //    public string MethodName { get; }
    //}
    /// <summary>
    /// SOAP服务配置
    /// </summary>
    public class SoapServiceConfiguration
    {
        public const int DEFAULT_CONCURRENCY_LIMIT = 10;
        internal SoapServiceConfiguration(string name)
        {
            Name = name;
        }
        /// <summary>
        /// 配置名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Obsolete]
        public Func<HttpClient>? ClientProvider { get; set; }
        /// <summary>
        /// 服务地址
        /// </summary>
        public string? Url { get; set; }
        /// <summary>
        /// SOAP版本
        /// </summary>
        public SoapVersion? Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? RequestNamespace { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ResponseNamespace { get; set; }
        /// <summary>
        /// 请求并发数量
        /// </summary>
        public int ConcurrencyLimit { get; set; }
    }
}


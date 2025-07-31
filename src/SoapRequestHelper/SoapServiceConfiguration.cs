using System;
using System.Net.Http;

namespace SoapRequestHelper;
/// <summary>
/// SOAP服务配置
/// </summary>
public class SoapServiceConfiguration
{
    internal const int DEFAULT_CONCURRENCY_LIMIT = 10;
    internal const int DEFAULT_QUEUE_CAPACITY = 100;
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
    /// 队列容量
    /// </summary>
    public int QueueCapacity { get; set; }
    /// <summary>
    /// 并发数量
    /// </summary>
    public int ConcurrencyLimit { get; set; }
}


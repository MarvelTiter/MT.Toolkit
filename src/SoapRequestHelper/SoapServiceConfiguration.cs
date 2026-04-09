using System.Diagnostics.CodeAnalysis;
using static SoapRequestHelper.SoapServiceConfiguration;

namespace SoapRequestHelper;
/// <summary>
/// SOAP服务配置
/// </summary>
public class SoapServiceConfiguration
{
    internal const int DEFAULT_CONCURRENCY_LIMIT = 20;
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



    /// <summary>
    /// 连接池配置对象
    /// </summary>
    public class DefaultHttpClientPoolSetting
    {
        /// <summary>
        /// 自定义HttpClient创建
        /// </summary>
        public Func<HttpClient>? ClientProvider { get; set; }
        /// <summary>
        /// 池大小，默认<see cref="DEFAULT_CONCURRENCY_LIMIT"/>
        /// </summary>
        public int HttpClientPoolSize { get; set; } = DEFAULT_CONCURRENCY_LIMIT;
        /// <summary>
        /// 等待实例超时时间，默认30s
        /// </summary>
        public TimeSpan WaitTimeout { get; set; } = TimeSpan.FromSeconds(30);

    }

    [NotNull] internal IHttpClientPool? HttpClientPool { get; set; }
    internal Func<IServiceProvider, IHttpClientPool>? ClientPoolFactory { get; set; }
    /// <summary>
    /// 配置HttpClient连接池
    /// </summary>
    public void UseHttpClientPool(IHttpClientPool pool)
    {
        HttpClientPool = pool;
    }
}

/// <summary>
/// 
/// </summary>
public static class SoapServiceConfigurationEx
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="action"></param>
    public static void UseDefaultHttpClientPool(this SoapServiceConfiguration configuration, Action<DefaultHttpClientPoolSetting>? action = null)
    {
        var setting = new DefaultHttpClientPoolSetting();
        action?.Invoke(setting);
        configuration.UseHttpClientPool(new DefaultHttpClientPool(setting));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="factory"></param>
    public static void UseHttpClientPool(this SoapServiceConfiguration configuration, Func<IServiceProvider, IHttpClientPool> factory)
    {
        configuration.ClientPoolFactory = factory;
    }
}


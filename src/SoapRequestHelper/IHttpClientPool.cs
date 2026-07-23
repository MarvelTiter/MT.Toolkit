namespace SoapRequestHelper;

/// <summary>
/// 自定义HttpClientPool
/// </summary>
public interface IHttpClientPool : IAsyncDisposable
{
    /// <summary>
    /// 获取HttpClient
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<HttpClient> GetAsync(HttpClientCreationContext context, CancellationToken cancellationToken = default);
    /// <summary>
    /// 归还HttpClient
    /// </summary>
    void Return(HttpClient client);
}




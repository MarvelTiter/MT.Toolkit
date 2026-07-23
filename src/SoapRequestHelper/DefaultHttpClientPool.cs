using System.Collections.Concurrent;
using static SoapRequestHelper.SoapServiceConfiguration;
namespace SoapRequestHelper;

/// <summary>
/// HttpClient实例池，大小与队列容量一致
/// </summary>
internal class DefaultHttpClientPool : IHttpClientPool
{
    private readonly ConcurrentQueue<HttpClient> pool = new();
    private readonly int poolSize;
    private bool isDisposed;
    private readonly object initLock = new();
    private readonly Func<HttpClient> httpClientFactory;
    private readonly TimeSpan timeout;
    /// <summary>
    /// 直接创建 HttpClient 实例池
    /// </summary>

    public DefaultHttpClientPool(DefaultHttpClientPoolSetting poolSetting)
    {
        httpClientFactory = poolSetting.ClientProvider ?? (() => new HttpClient());
        poolSize = poolSetting.HttpClientPoolSize;
        timeout = poolSetting.WaitTimeout;
        InitializePool();
    }

    private void InitializePool()
    {
        if (poolSize <= 0)
            throw new ArgumentException("Pool size must be greater than 0", nameof(poolSize));

        lock (initLock)
        {
            for (int i = 0; i < poolSize; i++)
            {
                var client = CreateHttpClient();
                pool.Enqueue(client);
            }
        }
    }

    private HttpClient CreateHttpClient()
    {
        var client = httpClientFactory.Invoke();
        return client;
    }

    /// <summary>
    /// 从池中获取一个 HttpClient 实例
    /// </summary>
    public async Task<HttpClient> GetAsync(HttpClientCreationContext context, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(isDisposed, this);
        if (pool.TryDequeue(out var client))
        {
            return client;
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(timeout);

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                if (pool.TryDequeue(out client))
                {
                    return client;
                }
                await Task.Delay(10, cts.Token).ConfigureAwait(false);
            }
            return CreateHttpClient();
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Timeout waiting for HttpClient from pool");
        }
    }

    /// <summary>
    /// 将 HttpClient 实例返回池中
    /// </summary>
    public void Return(HttpClient client)
    {
        if (isDisposed)
        {
            client.Dispose();
            return;
        }

        if (client == null)
            throw new ArgumentNullException(nameof(client));

        // 检查客户端是否仍然有效
        try
        {
            // 简单的健康检查：确保客户端没有被处置
            // 注意：HttpClient 没有 IsDisposed 属性，我们需要其他方式检查
            _ = client.BaseAddress; // 如果已处置，这会抛出异常
            TryEnqueue(client);
        }
        catch (ObjectDisposedException)
        {
            // 如果客户端已被处置，创建新的替代
            if (pool.Count < poolSize)
            {
                var newClient = CreateHttpClient();
                TryEnqueue(newClient);
            }
        }

        void TryEnqueue(HttpClient returnedClient)
        {
            if (pool.Count < poolSize)
            {
                pool.Enqueue(returnedClient);
            }
            else
            {
                returnedClient.Dispose();
            }
        }
    }

    /// <summary>
    /// 检查池的健康状态
    /// </summary>
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        if (pool.IsEmpty)
            return false;

        // 随机检查几个客户端
        var clientsToCheck = Math.Min(3, pool.Count);
        var healthyCount = 0;

        for (int i = 0; i < clientsToCheck; i++)
        {
            if (pool.TryDequeue(out var client))
            {
                try
                {
                    // 简单的健康检查：发送HEAD请求
                    using var request = new HttpRequestMessage(HttpMethod.Head, "");
                    using var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);

                    healthyCount++;
                    pool.Enqueue(client);
                }
                catch
                {
                    // 不健康的客户端，替换它
                    client.Dispose();
                    var newClient = CreateHttpClient();
                    pool.Enqueue(newClient);
                }
            }
        }

        return healthyCount == clientsToCheck;
    }

    /// <summary>
    /// 获取池的当前状态
    /// </summary>
    public HttpClientPoolStatus GetStatus()
    {
        return new HttpClientPoolStatus(poolSize, pool.Count, isDisposed);
    }

    /// <summary>
    /// 清理并重新初始化池
    /// </summary>
    public void Reset()
    {
        // 处置所有现有客户端
        while (pool.TryDequeue(out var client))
        {
            client.Dispose();
        }
        // 重新初始化池
        InitializePool();
    }



    public async ValueTask DisposeAsync()
    {
        if (isDisposed) return;

        isDisposed = true;

        while (pool.TryDequeue(out var client))
        {
            client.Dispose();
        }

        await Task.CompletedTask;
    }

    /// <summary>
    /// HttpClient池状态信息
    /// </summary>
    public readonly struct HttpClientPoolStatus(int totalSize, int availableCount, bool isDisposed)
    {
        public int TotalSize { get; } = totalSize;
        public int AvailableCount { get; } = availableCount;
        public bool IsDisposed { get; } = isDisposed;

        public int InUseCount => TotalSize - AvailableCount;
        public double UtilizationRate => TotalSize > 0 ? (double)InUseCount / TotalSize : 0;
    }
}




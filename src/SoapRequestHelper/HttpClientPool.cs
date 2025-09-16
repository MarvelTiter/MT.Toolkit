using System.Collections.Concurrent;
namespace SoapRequestHelper;

/// <summary>
/// HttpClient实例池，大小与队列容量一致
/// </summary>
internal class HttpClientPool : IAsyncDisposable
{
    private readonly ConcurrentQueue<HttpClient> pool = new();
    private readonly int poolSize;
    private bool isDisposed;
    private readonly object initLock = new();
    private readonly Func<HttpClient> httpClientFactory;
    /// <summary>
    /// 直接创建 HttpClient 实例池
    /// </summary>
    public HttpClientPool(int poolSize)
    {
        httpClientFactory = () => new HttpClient();
        this.poolSize = poolSize;
        InitializePool();
    }
    public HttpClientPool(Func<HttpClient> httpClientFactory, int poolSize)
    {
        this.httpClientFactory = httpClientFactory;
        this.poolSize = poolSize;
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
    public async Task<HttpClient> GetAsync(CancellationToken cancellationToken = default)
    {
        if (isDisposed)
            throw new ObjectDisposedException(nameof(HttpClientPool));

        if (pool.TryDequeue(out var client))
        {
            return client;
        }

        // 如果池为空（理论上不应该发生），等待或创建新实例
        // 这里选择等待其他客户端返回
        var tcs = new TaskCompletionSource<HttpClient>();
        var waitStartTime = DateTime.UtcNow;

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromSeconds(30)); // 30秒超时

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

            throw new TimeoutException("Timeout waiting for HttpClient from pool");
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
            pool.Enqueue(client);
        }
        catch (ObjectDisposedException)
        {
            // 如果客户端已被处置，创建新的替代
            var newClient = CreateHttpClient();
            pool.Enqueue(newClient);
        }
    }

    /// <summary>
    /// 执行一个使用 HttpClient 的操作
    /// </summary>
    public async Task<T> UseAsync<T>(Func<HttpClient, Task<T>> operation, CancellationToken cancellationToken = default)
    {
        var client = await GetAsync(cancellationToken).ConfigureAwait(false);

        try
        {
            return await operation(client).ConfigureAwait(false);
        }
        finally
        {
            Return(client);
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




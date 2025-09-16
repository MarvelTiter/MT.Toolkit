using Microsoft.Extensions.Configuration;
using System.Threading.Channels;
namespace SoapRequestHelper;

/// <summary>
/// 请求队列控制器的入参泛型限制
/// </summary>
/// <typeparam name="TOut"></typeparam>
public interface IHttpRequestChannelInput<TOut>
{
    /// <summary>
    /// 
    /// </summary>
    TaskCompletionSource<TOut> CompletionSource { get; }
}
/// <summary>
/// 请求队列控制器
/// </summary>
/// <typeparam name="TIn"></typeparam>
/// <typeparam name="TOut"></typeparam>
public class HttpRequestChannel<TIn, TOut> : IAsyncDisposable
    where TIn : IHttpRequestChannelInput<TOut>
{
    private readonly Channel<TIn>? channel;
    private readonly Task[] workerTasks = [];
    private readonly Func<TIn, Task<TOut>> handler;
    private readonly Func<TIn, ValueTask> writer;
    private readonly CancellationTokenSource cts = new();
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="capacity">队列容量</param>
    /// <param name="concurrentLimit">并发限制</param>
    /// <param name="handler">消息处理委托</param>
    public HttpRequestChannel(int capacity, int concurrentLimit, Func<TIn, Task<TOut>> handler)
    {
        if (capacity > 0)
        {
            channel = Channel.CreateBounded<TIn>(new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait,
                SingleReader = false,
                SingleWriter = false
            });

            // 启动工作线程
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(concurrentLimit);
            workerTasks = new Task[concurrentLimit];
            for (int i = 0; i < concurrentLimit; i++)
            {
                workerTasks[i] = Task.Run(ProcessRequestsAsync);
            }
            writer = WriteIntoChannelAsync;
        }
        else
        {
            writer = WriteDirectlyAsync;
        }

        this.handler = handler;
    }
    private async Task ProcessRequestsAsync()
    {
        if (channel == null) return;
        await foreach (var request in channel.Reader.ReadAllAsync(cts.Token))
        {
            try
            {
                var response = await handler(request);
                request.CompletionSource.SetResult(response);
            }
            catch (Exception ex)
            {
                request.CompletionSource.SetException(ex);
            }
        }
    }

    private ValueTask WriteIntoChannelAsync(TIn request)
    {
        if (channel == null) return ValueTask.CompletedTask;
        return channel.Writer.WriteAsync(request, cts.Token);
    }

    private ValueTask WriteDirectlyAsync(TIn request)
    {
        // 使用ConfigureAwait(false)避免同步上下文问题
        return HandleRequestAsync(request);

        async ValueTask HandleRequestAsync(TIn req)
        {
            try
            {
                var response = await handler(req).ConfigureAwait(false);
                req.CompletionSource.TrySetResult(response);
            }
            catch (Exception ex)
            {
                req.CompletionSource.TrySetException(ex);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public ValueTask WriteAsync(TIn request) => writer(request);

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <returns></returns>
    public async ValueTask DisposeAsync()
    {
        if (channel != null)
        {
            channel.Writer.Complete();
            await Task.WhenAll(workerTasks);
            foreach (var task in workerTasks)
            {
                task.Dispose();
            }
        }
        cts.Cancel();
        cts.Dispose();
        GC.SuppressFinalize(this);
    }
}
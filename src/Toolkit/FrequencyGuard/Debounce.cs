using System;
using System.Threading;
using System.Threading.Tasks;

namespace MT.Toolkit;

/// <summary>
/// 防抖器
/// </summary>
public class Debounce<T> : IDisposable
{
    private CancellationTokenSource? cts = null;
    private readonly Func<T, CancellationToken, Task> action;
    private readonly int delayMs;

    private Debounce(Action<T> action, int delayMs)
    {
        this.action = (value, _) =>
        {
            action(value);
            return Task.CompletedTask;
        };
        this.delayMs = delayMs;
    }

    private Debounce(Func<T, CancellationToken, Task> action, int delayMs)
    {
        this.action = action;
        this.delayMs = delayMs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    public async Task<bool> InvokeAsync(T value, CancellationToken cancellationToken = default)
    {
        cts?.Cancel();
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = cts.Token;
        try
        {
            await Task.Delay(delayMs, token);
            if (!token.IsCancellationRequested)
            {
                await action(value, token);
                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Debounce<T> Create(Action<T> action, int delayMs)
        => new(action, delayMs);

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Debounce<T> Create(Func<T, CancellationToken, Task> action, int delayMs)
       => new(action, delayMs);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// 无参防抖器
/// </summary>
public class Debounce : IDisposable
{
    private CancellationTokenSource? cts = null;
    private readonly Func<CancellationToken, Task> action;
    private readonly int delayMs;

    private Debounce(Action action, int delayMs)
    {
        this.action = _ =>
        {
            action();
            return Task.CompletedTask;
        };
        this.delayMs = delayMs;
    }

    private Debounce(Func<CancellationToken, Task> action, int delayMs)
    {
        this.action = action;
        this.delayMs = delayMs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    public async Task<bool> InvokeAsync(CancellationToken cancellationToken = default)
    {
        cts?.Cancel();
        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var token = cts.Token;
        try
        {
            await Task.Delay(delayMs, token);
            if (!token.IsCancellationRequested)
            {
                await action(token);
                return true;
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Debounce Create(Action action, int delayMs)
        => new(action, delayMs);

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Debounce Create(Func<CancellationToken, Task> action, int delayMs)
       => new(action, delayMs);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
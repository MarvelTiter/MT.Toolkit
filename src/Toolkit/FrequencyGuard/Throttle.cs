using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MT.Toolkit;

/// <summary>
/// 节流器
/// </summary>
public class Throttle<T> : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly Func<T, CancellationToken, Task> action;
    private readonly int delayMs;

    private Throttle(Action<T> action, int delayMs)
    {
        this.action = (value, _) =>
        {
            action(value);
            return Task.CompletedTask;
        };
        this.delayMs = delayMs;
    }

    private Throttle(Func<T, CancellationToken, Task> action, int delayMs)
    {
        this.action = action;
        this.delayMs = delayMs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InvokeAsync(T value, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await semaphoreSlim.WaitAsync(0, cancellationToken))
                return false;
            _ = ReleaseSemaphore(semaphoreSlim, delayMs);
            await action(value, cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task ReleaseSemaphore(SemaphoreSlim semaphore, int ms)
    {
        await Task.Delay(ms).ConfigureAwait(false);
        semaphore.Release();
    }

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Throttle<T> Create(Action<T> action, int delayMs)
        => new(action, delayMs);

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Throttle<T> Create(Func<T, CancellationToken, Task> action, int delayMs)
       => new(action, delayMs);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }
}


/// <summary>
/// 无参节流器
/// </summary>
public class Throttle : IDisposable
{
    private readonly SemaphoreSlim semaphoreSlim = new(1, 1);
    private readonly Func<CancellationToken, Task> action;
    private readonly int delayMs;

    private Throttle(Action action, int delayMs)
    {
        this.action = _ =>
        {
            action();
            return Task.CompletedTask;
        };
        this.delayMs = delayMs;
    }

    private Throttle(Func<CancellationToken, Task> action, int delayMs)
    {
        this.action = action;
        this.delayMs = delayMs;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InvokeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await semaphoreSlim.WaitAsync(0, cancellationToken))
                return false;
            _ = ReleaseSemaphore(semaphoreSlim, delayMs);
            await action(cancellationToken).ConfigureAwait(false);
            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    private static async Task ReleaseSemaphore(SemaphoreSlim semaphore, int ms)
    {
        await Task.Delay(ms).ConfigureAwait(false);
        semaphore.Release();
    }

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Throttle Create(Action action, int delayMs)
        => new(action, delayMs);

    /// <summary>
    /// 创建一个防抖器
    /// </summary>
    public static Throttle Create(Func<CancellationToken, Task> action, int delayMs)
       => new(action, delayMs);

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        semaphoreSlim.Dispose();
        GC.SuppressFinalize(this);
    }
}

using LoggerProviderExtensions.DbLogger;
using System.Collections.Concurrent;

namespace LoggerProviderExtensions;

/// <summary>
/// 
/// </summary>
/// <param name="content"></param>
/// <param name="isException"></param>
/// <param name="dt"></param>
public readonly struct DbLogItem(string content, bool isException, DateTimeOffset dt)
{
    /// <summary>
    /// 
    /// </summary>
    public string Content { get; } = content;
    /// <summary>
    /// 
    /// </summary>
    public bool IsException { get; } = isException;
    /// <summary>
    /// 
    /// </summary>
    public DateTimeOffset LogTime { get; } = dt;
}
internal class DatabaseLoggerProcessor
{
    private static Lazy<DatabaseLoggerProcessor>? _instance;
    private Lazy<IDbLogger>? _dbLogger;
    private Lazy<IDbLogger> DbLogger => _dbLogger ??= new Lazy<IDbLogger>(() =>
    {
        return LogConfig.DbLoggerFacotry?.Invoke() ?? throw new InvalidOperationException();
    });
    public static Lazy<DatabaseLoggerProcessor> GetDbLogger(DbLoggerOptions setting)
    {
        _instance ??= new Lazy<DatabaseLoggerProcessor>(() =>
        {
            return new DatabaseLoggerProcessor(setting);
        });
        return _instance;
    }
    private readonly ConcurrentQueue<DbLogItem> logQueue = new();
    public DbLoggerOptions LogConfig { get; set; }
    private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    private readonly Task dbTask;
    private DatabaseLoggerProcessor(DbLoggerOptions loggerSetting)
    {
        LogConfig = loggerSetting;
        var token = CancellationTokenSource.Token;
        dbTask = new Task(async () =>
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                while (logQueue.TryDequeue(out var logInfo))
                {
                    await DbLogger.Value.LogAsync(logInfo, token);
                }
            }
        }, token, TaskCreationOptions.LongRunning);
        dbTask.Start();
    }

    public void WriteLog(DbLogItem item)
    {
        logQueue.Enqueue(item);
    }

    ~DatabaseLoggerProcessor()
    {
        CancellationTokenSource.Cancel();
        dbTask.Wait();
        dbTask.Dispose();
    }
}

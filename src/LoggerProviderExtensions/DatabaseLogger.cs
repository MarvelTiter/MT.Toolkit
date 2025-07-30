using LoggerProviderExtensions.DbLogger;
using System.Collections.Concurrent;

namespace LoggerProviderExtensions;

public class DatabaseLogger 
{
    private static Lazy<DatabaseLogger>? _instance;
    private Lazy<IDbLogger>? _dbLogger;
    private Lazy<IDbLogger> DbLogger => _dbLogger ??= new Lazy<IDbLogger>(() =>
    {
        return LogConfig.DbLoggerFacotry?.Invoke() ?? throw new InvalidOperationException();
    });
    public static Lazy<DatabaseLogger> GetDbLogger(LoggerSetting setting)
    {
        _instance ??= new Lazy<DatabaseLogger>(() =>
        {
            return new DatabaseLogger(setting);
        });
        return _instance;
    }
    static readonly ConcurrentQueue<LogInfo> logQueue = new();
    public LoggerSetting LogConfig { get; set; }
    private readonly CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    Task dbTask;
    private DatabaseLogger(LoggerSetting loggerSetting)
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

    public void WriteLog(LogInfo logInfo)
    {
        logQueue.Enqueue(logInfo);
    }
}

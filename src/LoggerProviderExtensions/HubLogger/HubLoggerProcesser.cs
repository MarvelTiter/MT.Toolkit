using System.Text;
using System.Threading.Channels;

namespace LoggerProviderExtensions.HubLogger;

internal sealed class HubLoggerProcesser : IHubLoggerPublisher
{
    private static Lazy<HubLoggerProcesser>? _instance;

    public static Lazy<HubLoggerProcesser> GetHubLogger(HubLoggerOptions configuration)
    {
        _instance ??= new Lazy<HubLoggerProcesser>(() =>
        {
            return new HubLoggerProcesser(configuration);
        });
        return _instance;
    }
    private readonly Channel<StructuredLogEntry> channel;
    private readonly CancellationTokenSource cts;
    private bool disposedValue;
    private Task forwardTask;
    public event Action<StructuredLogEntry>? OnLog;

    private HubLoggerProcesser(HubLoggerOptions option)
    {
        var options = new BoundedChannelOptions(option.Capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleWriter = true,
            SingleReader = true
        };
        channel = Channel.CreateBounded<StructuredLogEntry>(options);
        cts = new CancellationTokenSource();
        forwardTask = new Task(async () =>
        {
            await foreach (var logEntry in channel.Reader.ReadAllAsync(cts.Token))
            {
                try
                {
                    OnLog?.Invoke(logEntry);
                }
                catch
                {
                    // Swallow exceptions from subscribers to avoid crashing the logger
                }
            }
        }, cts.Token, TaskCreationOptions.LongRunning);
        forwardTask.Start();
    }
    public void WriteLog(StructuredLogEntry logEntry)
    {
        ObjectDisposedException.ThrowIf(disposedValue, this);
        channel.Writer.TryWrite(logEntry);
    }

    ~HubLoggerProcesser()
    {
        channel.Writer.Complete();
        cts.Cancel();
        cts.Dispose();
    }
}

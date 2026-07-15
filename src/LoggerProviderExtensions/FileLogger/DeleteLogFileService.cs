#if NET6_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace LoggerProviderExtensions.FileLogger;

internal class DeleteLogFileService(IOptionsMonitor<FileLoggerOptions> config, ILogger<DeleteLogFileService> logger) : BackgroundService
{
    private readonly static Lazy<string> defaultLogFolder = new(() =>
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
    });

    private string LogFilePath => string.IsNullOrWhiteSpace(config.CurrentValue.LogFileFolder) ?
        defaultLogFolder.Value
        : config.CurrentValue.LogFileFolder;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!Directory.Exists(LogFilePath))
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            try
            {
                var files = Directory.EnumerateFiles(LogFilePath);
                var deadline = DateTime.Now.AddDays(-1 * config.CurrentValue.FileSavedDays);
                foreach (var file in files)
                {
                    var fileinfo = new FileInfo(file);
                    if (fileinfo.CreationTime < deadline)
                    {
                        File.Delete(file);
                    }
                }
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "日志文件清理服务异常");
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
#endif
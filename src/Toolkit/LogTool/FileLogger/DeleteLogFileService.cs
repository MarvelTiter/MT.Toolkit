#if NET6_0_OR_GREATER
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace MT.Toolkit.LogTool.FileLogger
{
    internal class DeleteLogFileService(IOptionsMonitor<LoggerSetting> config) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                int? savedDays = config.CurrentValue.FileSavedDays;
                if (!savedDays.HasValue || savedDays.Value < 0)
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }

                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logPath))
                {
                    await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                    continue;
                }
                try
                {
                    var files = Directory.EnumerateFiles(logPath);
                    var deadline = DateTime.Now.AddDays(-1 * savedDays.Value);
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
                catch (DirectoryNotFoundException)
                {
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
#endif
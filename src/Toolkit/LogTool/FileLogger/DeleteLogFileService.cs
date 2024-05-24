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
    internal class DeleteLogFileService(IOptionsMonitor<FileLoggerSetting> config) : BackgroundService
    {
        private readonly int? savedDays = config.CurrentValue.FileSavedDays;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!savedDays.HasValue)
            {
                return ;
            }

            if (savedDays.Value < 0)
            {
                return ;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var logPath = Path.Combine(Environment.CurrentDirectory, "logs");
                if (!Directory.Exists(logPath))
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
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
        }
    }
}

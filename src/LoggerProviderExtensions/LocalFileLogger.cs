using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace LoggerProviderExtensions;

public class LocalFileLogger 
{
    struct LogItem
    {
        public string Path { get; set; }
        public string Content { get; set; }
    }
    private static Lazy<LocalFileLogger>? _instance;

    public static Lazy<LocalFileLogger> GetFileLogger(LoggerSetting configuration)
    {
        _instance ??= new Lazy<LocalFileLogger>(() =>
        {
            return new LocalFileLogger(configuration);
        });
        return _instance;
    }

    public LoggerSetting LogConfig { get; set; }

    static readonly ConcurrentQueue<LogItem> logQueue = new ConcurrentQueue<LogItem>();

    private string separator = "----------------------------------------------------------------------------------------------------------------------";
    Task writeTask;
    CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    private LocalFileLogger(LoggerSetting configuration)
    {
        LogConfig = configuration;
        var token = CancellationTokenSource.Token;
        writeTask = new Task(() =>
       {
           while (!CancellationTokenSource.IsCancellationRequested)
           {
               // 阻塞1秒
               token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
               List<string[]> temp = new List<string[]>();
               
               while(logQueue.TryDequeue(out var logItem))
               {
                   string logPath = logItem.Path;
                   string logMergeContent = $"{logItem.Content}{separator}{Environment.NewLine}";
                   string[]? logArr = temp.FirstOrDefault(d => d[0].Equals(logPath));
                   if (logArr != null)
                   {
                       logArr[1] = string.Concat(logArr[1], logMergeContent);
                   }
                   else
                   {
                       logArr =
                       [
                           logPath,
                            logMergeContent
                       ];
                       temp.Add(logArr);
                   }
               }

               foreach (var item in temp)
               {
                   WriteText(item[0], item[1]);
               }
           }
       }, token, TaskCreationOptions.LongRunning);
        writeTask.Start();
    }
    public void WriteLog(LogInfo logInfo)
    {
        logQueue.Enqueue(new LogItem
        {
            Path = GetLogPath(LogConfig, logInfo.Category),
            Content = logInfo.FormatLogMessage()
        });
    }

    private static string GetLogPath(LoggerSetting setting, string? category)
    {
        string newFilePath;
        var logDir = setting.LogFileFolder ?? Path.Combine(Environment.CurrentDirectory, "logs");
        Directory.CreateDirectory(logDir);
        string extension = ".log";
        string fileNameNotExt = !string.IsNullOrEmpty(category) && setting.SaveByCategory ? $"{DateTime.Now:yyyy-MM-dd}_{category}_Part" : $"{DateTime.Now:yyyy-MM-dd}_Part";
        string fileNamePattern = string.Concat(fileNameNotExt, "*", extension);
        string[] filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly);

        if (filePaths.Length > 0)
        {
            int fileMaxLen = filePaths.Max(d => d.Length);
            string lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).First();
            if (new FileInfo(lastFilePath).Length > setting.LogFileSize)
            {
                var no = new Regex(@"(?<=Part)(\d+)").Match(Path.GetFileName(lastFilePath)).Value;
                var parse = int.TryParse(no, out int tempno);
                var formatno = $"{(parse ? tempno + 1 : tempno)}";
                var newFileName = string.Concat(fileNameNotExt, formatno, extension);
                newFilePath = Path.Combine(logDir, newFileName);
            }
            else
            {
                newFilePath = lastFilePath;
            }
        }
        else
        {
            var newFileName = string.Concat(fileNameNotExt, $"{0}", extension);
            newFilePath = Path.Combine(logDir, newFileName);
        }

        return newFilePath;
    }

    private static void WriteText(string logPath, string logContent)
    {
        try
        {
            if (!File.Exists(logPath))
            {
                File.CreateText(logPath).Close();
            }
            File.AppendAllText(logPath, logContent);
        }
        catch (Exception)
        {
            // ignored
            //Console.Write(ex.Message);
        }
    }

    ~LocalFileLogger()
    {
        CancellationTokenSource.Cancel();
        writeTask.Wait();
        writeTask.Dispose();
    }
}

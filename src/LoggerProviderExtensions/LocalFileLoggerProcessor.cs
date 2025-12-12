using LoggerProviderExtensions.FileLogger;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace LoggerProviderExtensions;

internal class LocalFileLoggerProcessor
{
    readonly struct LogItem(string path, string content)
    {
        public string Path { get; } = path;
        public string Content { get; } = content;
    }

    private static Lazy<LocalFileLoggerProcessor>? _instance;

    public static Lazy<LocalFileLoggerProcessor> GetFileLogger(FileLoggerOptions configuration)
    {
        _instance ??= new Lazy<LocalFileLoggerProcessor>(() =>
        {
            return new LocalFileLoggerProcessor(configuration);
        });
        return _instance;
    }

    public FileLoggerOptions LogConfig { get; set; }

    static readonly ConcurrentQueue<LogItem> logQueue = new();

    private const string SEPARATOR = "----------------------------------------------------------------------------------------------------------------------";
    private readonly Task writeTask;
    private readonly CancellationTokenSource CancellationTokenSource = new();
    private readonly Dictionary<string, StringBuilder> logBuffer = [];
    private LocalFileLoggerProcessor(FileLoggerOptions configuration)
    {
        LogConfig = configuration;
        var token = CancellationTokenSource.Token;
        writeTask = new Task(() =>
       {
           while (!CancellationTokenSource.IsCancellationRequested)
           {
               // 阻塞1秒
               token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));

               while (logQueue.TryDequeue(out var logItem))
               {
                   string logPath = logItem.Path;
                   if (!logBuffer.TryGetValue(logPath, out var sb))
                   {
                       logBuffer.Add(logPath, sb = new StringBuilder());
                   }
                   sb.Append(logItem.Content);
                   sb.Append(SEPARATOR);
                   sb.Append(Environment.NewLine);
               }

               foreach (var item in logBuffer)
               {
                   WriteText(item.Key, item.Value.ToString());
                   item.Value.Clear();
               }
           }
       }, token, TaskCreationOptions.LongRunning);
        writeTask.Start();
    }
    public void WriteLog(string category, string message)
    {
        logQueue.Enqueue(new LogItem(GetLogPath(LogConfig, category), message));
    }

    private static string GetLogPath(FileLoggerOptions setting, string? category)
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

    ~LocalFileLoggerProcessor()
    {
        CancellationTokenSource.Cancel();
        writeTask.Wait();
        writeTask.Dispose();
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool
{
    public class LocalFileLogger : ISimpleLogger
    {
        struct LogItem
        {
            public string Path { get; set; }
            public string Content { get; set; }
        }
        private static Lazy<ISimpleLogger> _instance = new(() => new LocalFileLogger());
        public static ISimpleLogger Instance => _instance.Value;
        public SimpleLoggerConfiguration? LogConfig { get; set; }

        static readonly ConcurrentQueue<LogItem> logQueue = new ConcurrentQueue<LogItem>();

        private string separator = "----------------------------------------------------------------------------------------------------------------------";
        CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        private LocalFileLogger()
        {
            var token = CancellationTokenSource.Token;
            var writeTask = new Task(obj =>
            {
                while (!token.IsCancellationRequested)
                {
                    // 阻塞1秒
                    token.WaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    List<string[]> temp = new List<string[]>();
                    foreach (var logItem in logQueue)
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

                        logQueue.TryDequeue(out var _);
                    }

                    foreach (var item in temp)
                    {
                        WriteText(item[0], item[1]);
                    }
                }
            }, null, token, TaskCreationOptions.LongRunning);
            writeTask.Start();
        }

        public void WriteLog(LogInfo logInfo)
        {
            logQueue.Enqueue(new LogItem
            {
                Path = GetLogPath(),
                Content = logInfo.FormatLogMessage()
            });
        }
        private static string GetLogPath()
        {
            string newFilePath;
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            Directory.CreateDirectory(logDir);
            string extension = ".log";
            string fileNameNotExt = $"{DateTime.Now:yyyy-MM-dd}_Part";
            string fileNamePattern = string.Concat(fileNameNotExt, "*", extension);
            string[] filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly);

            if (filePaths.Length > 0)
            {
                int fileMaxLen = filePaths.Max(d => d.Length);
                string lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).First();
                if (new FileInfo(lastFilePath).Length > 1 * 1024 * 1024)
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
        }
    }
}

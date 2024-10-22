using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    public interface IFileLoggerSetting
    {
        /// <summary>
        /// 文件保留天数，默认7天
        /// </summary>
        int FileSavedDays { get; set; }
        /// <summary>
        /// 日志保存目录
        /// </summary>
        string? LogFileFolder { get; set; }
        /// <summary>
        /// 文件大小，默认1m
        /// </summary>
        long LogFileSize { get; set; }
        /// <summary>
        /// 设置写入文件的日志级别
        /// </summary>
        void SetFileWriteLevel(string category, LogLevel logLevel);
        /// <summary>
        /// 设置写入文件的日志过滤
        /// </summary>
        void SetFileLogInfoFilter(Func<LogInfo, bool> filter);
    }
}

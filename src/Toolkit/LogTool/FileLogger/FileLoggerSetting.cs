using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.FileLogger
{
    public class FileLoggerSetting
    {
        public int? FileSavedDays { get; set; } = 7;
        public LogLevel WriteLevel { get; set; } = LogLevel.Error;
        /// <summary>
        /// 默认true
        /// </summary>
        public bool OnlyLogString { get; set; } = true;
        public Func<LogInfo, bool> CustomCheck { get; set; } = _ => true;

    }
}

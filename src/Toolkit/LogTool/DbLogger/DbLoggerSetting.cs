using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.DbLogger
{
    public class DbLoggerSetting
    {
        public LogLevel WriteLevel { get; set; } = LogLevel.Error;
        public bool LogString { get; set; } = false;
        public Func<LogInfo, bool> CustomCheck { get; set; } = _ => true;
    }
}

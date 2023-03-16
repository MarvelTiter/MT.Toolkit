using System;

namespace MT.Toolkit.LogTool
{
	public class ConsoleLogger : ISimpleLogger
	{
		public SimpleLoggerConfiguration LogConfig { get; set; }

		public void WriteLog(LogInfo logInfo)
		{
			//switch (logInfo.LogLevel)
			//{
			//    case SimpleLogLevel.Information:
			//        Console.ForegroundColor = ConsoleColor.Gray;
			//        break;
			//    case SimpleLogLevel.Debug:
			//        Console.ForegroundColor = ConsoleColor.DarkBlue;
			//        break;
			//    case SimpleLogLevel.Warning:
			//        Console.ForegroundColor = ConsoleColor.Yellow;
			//        break;
			//    case SimpleLogLevel.Error:
			//        Console.ForegroundColor = ConsoleColor.Red;
			//        break;
			//    case SimpleLogLevel.Critical:
			//        Console.ForegroundColor = ConsoleColor.DarkRed;
			//        break;
			//    default:
			//        break;
			//}

			if (!LogConfig.ConsoleColor.TryGetValue(logInfo.LogLevel, out var color))
			{
				color = ConsoleColor.White;
			}
			Console.ForegroundColor = color;
			Console.Write(logInfo.LogHeader());
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write(logInfo.LogCategory());
			Console.Write(logInfo.LogBody());
		}
	}
}

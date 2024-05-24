
#if !NET40_OR_GREATER
using System;
using Microsoft.Extensions.Logging;

namespace MT.Toolkit.LogTool.LogExtension
{

	internal class InternalLogger : ILogger
	{
		private readonly string name;
		private readonly Func<SimpleLoggerConfiguration> getCurrentConfig;
		private readonly Logger logger;
		public InternalLogger(string name, Func<SimpleLoggerConfiguration> getCurrentConfig)
		{
			this.name = name;
			this.getCurrentConfig = getCurrentConfig;
			logger = new Logger(this.getCurrentConfig());
		}
		public IDisposable BeginScope<TState>(TState state) where TState : notnull
		{
			return NullScope.Instance;
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return logger.IsEnabled(logLevel);
		}

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel))
			{
				return;
			}
			logger.Log(logLevel, state, formatter, name, eventId.Id, eventId.Name, exception);
		}

		internal sealed class NullScope : IDisposable
		{
			public static NullScope Instance { get; } = new NullScope();


			private NullScope()
			{
			}

			public void Dispose()
			{
			}
		}
	}
}
#endif

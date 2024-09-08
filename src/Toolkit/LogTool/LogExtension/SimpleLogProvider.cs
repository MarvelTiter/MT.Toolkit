#if !NET40_OR_GREATER

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MT.Toolkit.LogTool;

using System;
using System.Collections.Concurrent;

namespace MT.Toolkit.LogTool.LogExtension
{

	[ProviderAlias("SimgleLog")]
	public class SimpleLogProvider : ILoggerProvider
	{
		private bool disposedValue;
		private readonly IDisposable? _onChangeToken;
		private LoggerSetting _currentConfig;
		private readonly ConcurrentDictionary<string, InternalLogger> _loggers =
			new ConcurrentDictionary<string, InternalLogger>(StringComparer.OrdinalIgnoreCase);

		public SimpleLogProvider(IOptionsMonitor<LoggerSetting> config)
		{
			_currentConfig = config.CurrentValue;
			_onChangeToken = config.OnChange(updatedConfig => _currentConfig = updatedConfig);
		}

		public ILogger CreateLogger(string categoryName) =>
			_loggers.GetOrAdd(categoryName, name => new InternalLogger(name, GetCurrentConfig));

		LoggerSetting GetCurrentConfig() => _currentConfig;

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// 释放托管状态(托管对象)
					_loggers.Clear();
					_onChangeToken?.Dispose();
				}

				// 释放未托管的资源(未托管的对象)并重写终结器
				// 将大型字段设置为 null
				disposedValue = true;
			}
		}

		// // 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
		// ~SimpleLogProvider()
		// {
		//     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
		//     Dispose(disposing: false);
		// }

		public void Dispose()
		{
			// 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
#endif

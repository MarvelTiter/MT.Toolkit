#if !NET40_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.LogTool.LogExtension
{


	public static class SerivceExtension
	{
		public static ILoggingBuilder AddSimpleLogger(this ILoggingBuilder builder)
		{
			builder.ClearProviders();
			builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, SimpleLogProvider>());
			LoggerProviderOptions.RegisterProviderOptions<SimpleLoggerConfiguration, SimpleLogProvider>(builder.Services);
			return builder;
		}
		public static ILoggingBuilder AddSimpleLogger(this ILoggingBuilder builder, Action<SimpleLoggerConfiguration> config)
		{
			builder.AddSimpleLogger();
			builder.Services.Configure(config);
			//config();
			return builder;
		}
	}
}
#endif

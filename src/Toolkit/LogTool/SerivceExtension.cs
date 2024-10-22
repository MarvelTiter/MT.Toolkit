#if NET6_0_OR_GREATER
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MT.Toolkit.LogTool.DbLogger;
using MT.Toolkit.LogTool.FileLogger;
using System;

namespace MT.Toolkit.LogTool
{
    public static class SerivceExtension
    {
        [Obsolete]
        public static ILoggingBuilder AddSimpleLogger(this ILoggingBuilder builder)
        {
            return builder;
        }
        [Obsolete]
        public static ILoggingBuilder AddSimpleLogger(this ILoggingBuilder builder, Action<LoggerSetting> config)
        {
            builder.AddSimpleLogger();
            builder.Services.Configure(config);
            //config();
            return builder;
        }

        public static ILoggingBuilder AddLocalFileLogger(this ILoggingBuilder builder, Action<IFileLoggerSetting>? config = null)
        {
            config ??= c => { };
            builder.Services.Configure<LoggerSetting>(config);
            builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
            builder.Services.AddHostedService<DeleteLogFileService>();
            return builder;
        }

        public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, Action<IDbLoggerSetting>? config = null)
        {
            config ??= c => { };
            builder.Services.Configure<LoggerSetting>(config);
            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider>();
            return builder;
        }
    }
}
#endif
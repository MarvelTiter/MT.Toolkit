using LoggerProviderExtensions.DbLogger;
using LoggerProviderExtensions.FileLogger;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoggerProviderExtensions;

public static class SerivceExtension
{
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
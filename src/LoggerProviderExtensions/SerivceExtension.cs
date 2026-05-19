using LoggerProviderExtensions.BindHelper;
using LoggerProviderExtensions.DbLogger;
using LoggerProviderExtensions.FileLogger;
using LoggerProviderExtensions.HubLogger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace LoggerProviderExtensions;

/// <summary>
/// 
/// </summary>
public static class SerivceExtension
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddHubLogger(this ILoggingBuilder builder, Action<HubLoggerOptions>? config = null)
    {
        builder.AddOptions<HubLoggerOptions, HubLoggerConfigureOptions, HubLoggerOptionsChangeTokenSource<HubLoggerOptions>>();
        builder.Services.AddSingleton<HubLoggerProvider>();
        builder.Services.AddSingleton<ILoggerProvider, HubLoggerProvider>(sp =>
        {
            return sp.GetRequiredService<HubLoggerProvider>();
        });
        builder.Services.AddSingleton<IHubLoggerPublisher>(sp =>
        {
            var p = sp.GetRequiredService<HubLoggerProvider>();
            return p.PL;
        });
        if (config is not null)
        {
            builder.Services.Configure(config);
        }
        return builder;
    }

    /// <summary>
    /// 启用文件日志, 详细可配置参数<see cref="FileLoggerOptions"/>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddLocalFileLogger(this ILoggingBuilder builder, Action<FileLoggerOptions>? config = null)
    {
        builder.AddOptions<FileLoggerOptions, FileLoggerConfigureOptions, FileLoggerOptionsChangeTokenSource<FileLoggerOptions>>();
        builder.Services.AddSingleton<ILoggerProvider, FileLoggerProvider>();
        builder.Services.AddHostedService<DeleteLogFileService>();
        if (config is not null)
        {
            builder.Services.Configure(config);
        }
        return builder;
    }

    /// <summary>
    /// 启用数据库日志
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static ILoggingBuilder AddDbLogger(this ILoggingBuilder builder, Action<DbLoggerOptions>? config = null)
    {
        builder.AddOptions<DbLoggerOptions, DbLoggerConfigureOptions, DbLoggerOptionsChangeTokenSource<DbLoggerOptions>>();
        builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider>();
        if (config is not null)
        {
            builder.Services.Configure(config);
        }
        return builder;
    }

    private static void AddOptions<TOptions,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TConfigureOptions,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TOptionsChangeTokenSource>(this ILoggingBuilder builder)
        where TOptions : class
        where TConfigureOptions : class, IConfigureOptions<TOptions>
        where TOptionsChangeTokenSource : ConfigurationChangeTokenSource<TOptions>
    {
        builder.Services.TryAddSingleton<IConfigureOptions<TOptions>, TConfigureOptions>();
        builder.Services.TryAddSingleton<IOptionsChangeTokenSource<TOptions>, TOptionsChangeTokenSource>();
    }

    internal static IConfiguration GetFileLoggerConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("Logging:LocalFile");
    }
    internal static IConfiguration GetDbLoggerConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("Logging:DatabaseLog");
    }
    internal static IConfiguration GetHubLoggerConfiguration(this IConfiguration configuration)
    {
        return configuration.GetSection("Logging:Hub");
    }
}
#region FileLogger Options Support Classes
[UnsupportedOSPlatform("browser")]
internal sealed class FileLoggerOptionsChangeTokenSource<TOptions>(IConfiguration configuration) : ConfigurationChangeTokenSource<TOptions>(configuration.GetFileLoggerConfiguration())
{
}

[method: UnsupportedOSPlatform("browser")]
internal sealed class FileLoggerConfigureOptions(IConfiguration configuration) : IConfigureOptions<FileLoggerOptions>
{
    private readonly IConfiguration configuration = configuration.GetFileLoggerConfiguration();
    public void Configure(FileLoggerOptions options)
    {
        configuration.Bind_FileLoggerOptions(options);
    }
}
#endregion

#region DbLogger Options Support Classes
[UnsupportedOSPlatform("browser")]
internal sealed class DbLoggerOptionsChangeTokenSource<TOptions>(IConfiguration configuration) : ConfigurationChangeTokenSource<TOptions>(configuration.GetDbLoggerConfiguration())
{
}

[method: UnsupportedOSPlatform("browser")]
internal sealed class DbLoggerConfigureOptions(IConfiguration configuration) : IConfigureOptions<DbLoggerOptions>
{
    private readonly IConfiguration configuration = configuration.GetDbLoggerConfiguration();

    public void Configure(DbLoggerOptions options)
    {
        configuration.Bind_DbLoggerOptions(options);
    }
}
#endregion

#region HubLogger Options Support Classes
[UnsupportedOSPlatform("browser")]
internal sealed class HubLoggerOptionsChangeTokenSource<TOptions>(IConfiguration configuration) : ConfigurationChangeTokenSource<TOptions>(configuration.GetDbLoggerConfiguration())
{
}

internal sealed class HubLoggerConfigureOptions(IConfiguration configuration) : IConfigureOptions<HubLoggerOptions>
{
    private readonly IConfiguration configuration = configuration.GetHubLoggerConfiguration();

    public void Configure(HubLoggerOptions options)
    {

    }
}
#endregion
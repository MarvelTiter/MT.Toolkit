using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace LoggerProviderExtensions.BindHelper;

internal static class BindingExtensions
{
    public static void Bind_FileLoggerOptions(this IConfiguration configuration, object? instance)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (instance is null)
        {
            return;
        }

        var typedObj = (global::LoggerProviderExtensions.FileLogger.FileLoggerOptions)instance;
        BindCore(configuration, ref typedObj, defaultValueIfNotFound: false, binderOptions: null);
    }

    /// <summary>Attempts to bind the given object instance to configuration values by matching property names against configuration keys recursively.</summary>
    public static void Bind_DbLoggerOptions(this IConfiguration configuration, object? instance)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        if (instance is null)
        {
            return;
        }

        var typedObj = (global::LoggerProviderExtensions.DbLogger.DbLoggerOptions)instance;
        BindCore(configuration, ref typedObj, defaultValueIfNotFound: false, binderOptions: null);
    }

    private readonly static Lazy<HashSet<string>> s_configKeys_FileLoggerOptions = new(() => new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "FileSavedDays", "LogFileFolder", "LogFileSize", "SaveByCategory", "SaveByLevel", "MinLevel", "IncludeScopes", "TimestampFormat", "UseUtcTimestamp", "Structured" });
    private readonly static Lazy<HashSet<string>> s_configKeys_DbLoggerOptions = new(() => new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "MinLevel", "IncludeScopes", "TimestampFormat", "UseUtcTimestamp", "Structured" });

    public static void BindCore(IConfiguration configuration, ref global::LoggerProviderExtensions.FileLogger.FileLoggerOptions instance, bool defaultValueIfNotFound, BinderOptions? binderOptions)
    {
        ValidateConfigurationKeys(typeof(global::LoggerProviderExtensions.FileLogger.FileLoggerOptions), s_configKeys_FileLoggerOptions, configuration, binderOptions);

        if (TryGetConfigurationValue(configuration, key: "FileSavedDays", out string? value0))
        {
            if (!string.IsNullOrEmpty(value0))
            {
                instance.FileSavedDays = ParseInt(value0, configuration.GetSection("FileSavedDays").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.FileSavedDays = instance.FileSavedDays;
        }

        if (TryGetConfigurationValue(configuration, key: "LogFileFolder", out string? value1))
        {
            instance.LogFileFolder = value1;
        }
        else if (defaultValueIfNotFound)
        {
            var currentValue = instance.LogFileFolder;
            if (currentValue is not null)
            {
                instance.LogFileFolder = currentValue;
            }
        }

        if (TryGetConfigurationValue(configuration, key: "LogFileSize", out string? value2))
        {
            if (!string.IsNullOrEmpty(value2))
            {
                instance.LogFileSize = ParseLong(value2, configuration.GetSection("LogFileSize").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.LogFileSize = instance.LogFileSize;
        }

        if (TryGetConfigurationValue(configuration, key: "SaveByCategory", out string? value3))
        {
            if (!string.IsNullOrEmpty(value3))
            {
                instance.SaveByCategory = ParseBool(value3, configuration.GetSection("SaveByCategory").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.SaveByCategory = instance.SaveByCategory;
        }

        if (TryGetConfigurationValue(configuration, key: "SaveByLevel", out string? value4))
        {
            if (!string.IsNullOrEmpty(value4))
            {
                instance.SaveByLevel = ParseBool(value4, configuration.GetSection("SaveByLevel").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.SaveByLevel = instance.SaveByLevel;
        }

        if (TryGetConfigurationValue(configuration, key: "MinLevel", out string? value5))
        {
            if (!string.IsNullOrEmpty(value5))
            {
                instance.MinLevel = ParseEnum<global::Microsoft.Extensions.Logging.LogLevel>(value5, configuration.GetSection("MinLevel").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.MinLevel = instance.MinLevel;
        }

        if (TryGetConfigurationValue(configuration, key: "IncludeScopes", out string? value6))
        {
            if (!string.IsNullOrEmpty(value6))
            {
                instance.IncludeScopes = ParseBool(value6, configuration.GetSection("IncludeScopes").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.IncludeScopes = instance.IncludeScopes;
        }

        if (TryGetConfigurationValue(configuration, key: "TimestampFormat", out string? value7))
        {
            instance.TimestampFormat = value7;
        }
        else if (defaultValueIfNotFound)
        {
            var currentValue = instance.TimestampFormat;
            if (currentValue is not null)
            {
                instance.TimestampFormat = currentValue;
            }
        }

        if (TryGetConfigurationValue(configuration, key: "UseUtcTimestamp", out string? value8))
        {
            if (!string.IsNullOrEmpty(value8))
            {
                instance.UseUtcTimestamp = ParseBool(value8, configuration.GetSection("UseUtcTimestamp").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.UseUtcTimestamp = instance.UseUtcTimestamp;
        }

        if (TryGetConfigurationValue(configuration, key: "Structured", out string? value9))
        {
            if (!string.IsNullOrEmpty(value9))
            {
                instance.Structured = ParseBool(value9, configuration.GetSection("Structured").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.Structured = instance.Structured;
        }

    }

    public static void BindCore(IConfiguration configuration, ref global::LoggerProviderExtensions.DbLogger.DbLoggerOptions instance, bool defaultValueIfNotFound, BinderOptions? binderOptions)
    {
        ValidateConfigurationKeys(typeof(global::LoggerProviderExtensions.DbLogger.DbLoggerOptions), s_configKeys_DbLoggerOptions, configuration, binderOptions);

        if (TryGetConfigurationValue(configuration, key: "MinLevel", out string? value13))
        {
            if (!string.IsNullOrEmpty(value13))
            {
                instance.MinLevel = ParseEnum<global::Microsoft.Extensions.Logging.LogLevel>(value13, configuration.GetSection("MinLevel").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.MinLevel = instance.MinLevel;
        }

        if (TryGetConfigurationValue(configuration, key: "IncludeScopes", out string? value14))
        {
            if (!string.IsNullOrEmpty(value14))
            {
                instance.IncludeScopes = ParseBool(value14, configuration.GetSection("IncludeScopes").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.IncludeScopes = instance.IncludeScopes;
        }

        if (TryGetConfigurationValue(configuration, key: "TimestampFormat", out string? value15))
        {
            instance.TimestampFormat = value15;
        }
        else if (defaultValueIfNotFound)
        {
            var currentValue = instance.TimestampFormat;
            if (currentValue is not null)
            {
                instance.TimestampFormat = currentValue;
            }
        }

        if (TryGetConfigurationValue(configuration, key: "UseUtcTimestamp", out string? value16))
        {
            if (!string.IsNullOrEmpty(value16))
            {
                instance.UseUtcTimestamp = ParseBool(value16, configuration.GetSection("UseUtcTimestamp").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.UseUtcTimestamp = instance.UseUtcTimestamp;
        }

        if (TryGetConfigurationValue(configuration, key: "Structured", out string? value17))
        {
            if (!string.IsNullOrEmpty(value17))
            {
                instance.Structured = ParseBool(value17, configuration.GetSection("Structured").Path);
            }
        }
        else if (defaultValueIfNotFound)
        {
            instance.Structured = instance.Structured;
        }
    }

    /// <summary>Tries to get the configuration value for the specified key.</summary>
    public static bool TryGetConfigurationValue(IConfiguration configuration, string key, out string? value)
    {
        if (configuration is ConfigurationSection section)
        {
            return section.TryGetValue(key, out value);
        }

        value = key != null ? configuration[key] : configuration is IConfigurationSection sec ? sec.Value : null;
        return value != null;
    }


    /// <summary>If required by the binder options, validates that there are no unknown keys in the input configuration object.</summary>
    public static void ValidateConfigurationKeys(Type type, Lazy<HashSet<string>> keys, IConfiguration configuration, BinderOptions? binderOptions)
    {
        if (binderOptions?.ErrorOnUnknownConfiguration is true)
        {
            List<string>? temp = null;

            foreach (IConfigurationSection section in configuration.GetChildren())
            {
                if (!keys.Value.Contains(section.Key))
                {
                    (temp ??= new List<string>()).Add($"'{section.Key}'");
                }
            }

            if (temp is not null)
            {
                throw new InvalidOperationException($"'ErrorOnUnknownConfiguration' was set on the provided BinderOptions, but the following properties were not found on the instance of {type}: {string.Join(", ", temp)}");
            }
        }
    }

    public static IConfiguration? AsConfigWithChildren(IConfiguration configuration)
    {
        foreach (IConfigurationSection _ in configuration.GetChildren())
        {
            return configuration;
        }
        return null;
    }

    public static T ParseEnum<T>(string value, string? path) where T : struct
    {
        try
        {
            return Enum.Parse<T>(value, ignoreCase: true);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to convert configuration value '{value ?? "null"}' at '{path}' to type '{typeof(T)}'.", exception);
        }
    }

    public static int ParseInt(string value, string? path)
    {
        try
        {
            return int.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to convert configuration value '{value ?? "null"}' at '{path}' to type '{typeof(int)}'.", exception);
        }
    }

    public static long ParseLong(string value, string? path)
    {
        try
        {
            return long.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to convert configuration value '{value ?? "null"}' at '{path}' to type '{typeof(long)}'.", exception);
        }
    }

    public static bool ParseBool(string value, string? path)
    {
        try
        {
            return bool.Parse(value);
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException($"Failed to convert configuration value '{value ?? "null"}' at '{path}' to type '{typeof(bool)}'.", exception);
        }
    }
}



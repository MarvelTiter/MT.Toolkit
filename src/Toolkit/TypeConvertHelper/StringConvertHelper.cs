using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace MT.Toolkit.TypeConvertHelper;

#pragma warning disable
/// <summary>
/// 
/// </summary>
public static class StringConvertHelper
{
    static readonly ConcurrentDictionary<Type, MethodInfo?> tryParseCache = [];
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    [Obsolete("use StringExtensions.IsNumeric{T}(string?, out T?) instead")]
    public static T? Parse<T>(this string input)
    {
        return (T?)Parse(input, typeof(T));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [Obsolete("use StringExtensions.IsNumeric{T}(string?, out T?) instead")]
    public static object? Parse(this string input, Type type)
    {
        if (type == typeof(string))
        {
            return input.Trim();
        }
        type = Nullable.GetUnderlyingType(type) ?? type;
        var method = tryParseCache.GetOrAdd(type, t =>
        {
            return t.GetMethod("TryParse", [typeof(string), t.MakeByRefType()]);
        });
        if (method == null)
        {
            return default;
        }
        object?[] parameters = [input, default];
        var r = method.Invoke(null, parameters);
        if (r is bool b && b == true)
        {
            return parameters[1];
        }
        return default;
    }

}

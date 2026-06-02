using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MT.Toolkit.StringExtension;

/// <summary>
/// <see cref="string"/>扩展方法
/// </summary>
public static partial class StringExtensions
{
    /// <summary>
    /// Returns the original string if the specified condition evaluates to true; otherwise, returns an empty string.
    /// </summary>
    /// <returns>The original string if the condition is true; otherwise, an empty string.</returns>
    public static string If(this string? self, Func<bool> condition)
    {
        return self.If(condition.Invoke());
    }
    /// <summary>
    /// Returns the original string if the specified condition evaluates to true; otherwise, returns an empty string.
    /// </summary>
    /// <returns>The original string if the condition is true; otherwise, an empty string.</returns>
    public static string If(this string? self, bool condition)
    {
        if (string.IsNullOrEmpty(self))
        {
            return string.Empty;
        }
        if (condition)
        {
            return self!;
        }
        return string.Empty;
    }

    /// <summary>
    /// 字符串是否合法，如果rule不为null，则根据rule判断字符串是否合法，否则只要字符串不为null或空白就合法
    /// </summary>
    /// <returns></returns>
    public static bool IsEnable(this string? self, Func<string, bool>? rule = null)
    {
        if (string.IsNullOrWhiteSpace(self))
        {
            return false;
        }
        return rule?.Invoke(self!) ?? true;
    }

    /// <summary>
    /// 是否是数字字符串，如果是数字字符串则将其转换为对应的数字类型，否则返回默认值，优先使用.NET 8.0的INumber接口进行转换，如果不支持则使用正则表达式判断字符串是否合法并进行转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool IsNumeric<T>(this string? self,
#if NET6_0_OR_GREATER
        [NotNullWhen(true)]
#endif
    out T? value)
#if NET8_0_OR_GREATER
        where T : INumber<T>
#endif
    {
#if NET8_0_OR_GREATER
        return T.TryParse(self, null, out value);
#else
    var match = self.IsNumeric();
        if (match)
        {
            value = (T)StringTo(self!, typeof(T));
        }
        else
            value = default!;
        return match;
#endif
    }

    /// <summary>
    /// 检测是否是数字
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    public static bool IsNumeric(this string? self)
    {
        if (string.IsNullOrEmpty(self))
        {
            return false;
        }
#if NET7_0_OR_GREATER
        var match = NumberMatchRegex().IsMatch(self);
#else
        var match = Regex.IsMatch(self, @"([1-9]\d*\.?\d*)|(0\.\d*[1-9])");
#endif
        return match;
    }
#if NET7_0_OR_GREATER
    [GeneratedRegex(@"([1-9]\d*\.?\d*)|(0\.\d*[1-9])")]
    private static partial Regex NumberMatchRegex();
#endif

    /// <summary>
    /// 追加字符串到StringBuilder，如果字符串不合法则不追加
    /// </summary>
    /// <param name="str"></param>
    /// <param name="builder"></param>
    public static void AppendTo(this string str, StringBuilder builder)
    {
        if (str.IsEnable())
            builder.AppendLine(str);
    }

    /// <summary>
    /// 将字符串转为 [S]Byte | [U]Int16 | [U]Int32 | [U]Int64 | Single | Double | Decimal
    /// </summary>
    public static T StringTo<T>(this string? content) => (T)content.StringTo(typeof(T));
    /// <summary>
    /// 将字符串转为 [S]Byte | [U]Int16 | [U]Int32 | [U]Int64 | Single | Double | Decimal
    /// </summary>
    public static object StringTo(this string? content, Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.SByte:
                _ = sbyte.TryParse(content, out var sbyteValue);
                return sbyteValue;
            case TypeCode.Byte:
                _ = byte.TryParse(content, out byte byteValue);
                return byteValue;
            case TypeCode.Int16:
                _ = short.TryParse(content, out short shortValue);
                return shortValue;
            case TypeCode.UInt16:
                _ = ushort.TryParse(content, out ushort ushortValue);
                return ushortValue;
            case TypeCode.Int32:
                _ = int.TryParse(content, out int intValue);
                return intValue;
            case TypeCode.UInt32:
                _ = uint.TryParse(content, out uint uintValue);
                return uintValue;
            case TypeCode.Int64:
                _ = long.TryParse(content, out long longValue);
                return longValue;
            case TypeCode.UInt64:
                _ = ulong.TryParse(content, out ulong ulongValue);
                return ulongValue;
            case TypeCode.Single:
                _ = float.TryParse(content, out float floatValue);
                return floatValue;
            case TypeCode.Double:
                _ = double.TryParse(content, out double doubleValue);
                return doubleValue;
            case TypeCode.Decimal:
                _ = decimal.TryParse(content, out decimal decimalValue);
                return decimalValue;
            default:
                throw new NotSupportedException("只支持数字类型");
        }
    }
}
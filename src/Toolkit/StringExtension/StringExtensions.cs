using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MT.Toolkit.StringExtension;

public static partial class StringExtensions
{
    public static string If(this string? self, Func<bool> condition)
    {
        return self.If(condition.Invoke());
    }

    public static string If(this string? self, bool condition)
    {
        if (string.IsNullOrEmpty(self))
        {
            return string.Empty;
        }
        if (condition)
        {
            return self;
        }
        return string.Empty;
    }

    public static bool IsEnable(this string? self, Func<string, bool>? rule = null)
    {
        if (string.IsNullOrEmpty(self))
        {
            return false;
        }

        var b = rule?.Invoke(self);
        return b.HasValue && b.Value;
    }

    public static bool IsNumeric<T>(this string? self, out T value)
    {
        var match = self.IsNumeric();
        if (match)
        {
            value = (T)StringTo(self!, typeof(T));
        }
        else
            value = default!;

        return match;

    }

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
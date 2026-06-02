using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.ExpressionHelper;

internal class DataTypeConvert
{
    public static Expression GetConversionExpression(Expression source,
        Type sourceType,
        Type targetType)
    {
        if (ReferenceEquals(sourceType, targetType))
        {
            return Expression.Convert(source, sourceType);
        }
        else if (ReferenceEquals(sourceType, typeof(string)))
        {
            // XXX.Parse()               
            return GetParseExpression(source, targetType, CultureInfo.CurrentCulture);
        }
        else if (ReferenceEquals(targetType, typeof(string)))
        {
            // XXX.ToString()
            return Expression.Call(source, typeof(Convert).GetMethod("ToString", [sourceType])!);
        }
        else if (ReferenceEquals(targetType, typeof(bool)))
        {
            MethodInfo ToBooleanMethod = typeof(Convert).GetMethod("ToBoolean", [sourceType])!;
            return Expression.Call(ToBooleanMethod, source);
        }
        else if (ReferenceEquals(sourceType, typeof(byte[])))
        {
            return GetArrayHandlerExpression(source, targetType);
        }
        else
        {
            return ConvertTypeExpression(source, sourceType, targetType);
        }
    }
    private static Expression GetArrayHandlerExpression(Expression sourceExpression, Type targetType)
    {
        Expression TargetExpression = default!;
        if (ReferenceEquals(targetType, typeof(byte[])))
        {
            TargetExpression = sourceExpression;
        }
        else if (ReferenceEquals(targetType, typeof(MemoryStream)))
        {
            ConstructorInfo ConstructorInfo = typeof(MemoryStream).GetConstructor([typeof(byte[])])!;
            TargetExpression = Expression.New(ConstructorInfo, sourceExpression);
        }
        else
        {
            throw new ArgumentException("Cannot convert a byte array to " + targetType.Name);
        }
        return TargetExpression;
    }

    private static Expression GetParseExpression(Expression SourceExpression, Type TargetType, CultureInfo Culture)
    {
        Type UnderlyingType = Nullable.GetUnderlyingType(TargetType) ?? TargetType;
        if (UnderlyingType.IsEnum)
        {
            MethodCallExpression ParsedEnumExpression = GetEnumParseExpression(SourceExpression, UnderlyingType);
            //Enum.Parse returns an object that needs to be unboxed
            return Expression.Unbox(ParsedEnumExpression, TargetType);
        }
        else
        {
            Expression valueExp = default!;
            valueExp = UnderlyingType.FullName switch
            {
                "System.Byte" => NumberParseExpression<byte>(SourceExpression, Culture),
                "System.UInt16" => NumberParseExpression<ushort>(SourceExpression, Culture),
                "System.UInt32" => NumberParseExpression<uint>(SourceExpression, Culture),
                "System.UInt64" => NumberParseExpression<ulong>(SourceExpression, Culture),
                "System.SByte" => NumberParseExpression<sbyte>(SourceExpression, Culture),
                "System.Int16" => NumberParseExpression<short>(SourceExpression, Culture),
                "System.Int32" => NumberParseExpression<int>(SourceExpression, Culture),
                "System.Int64" => NumberParseExpression<long>(SourceExpression, Culture),
                "System.Double" => NumberParseExpression<double>(SourceExpression, Culture),
                "System.Decimal" => NumberParseExpression<decimal>(SourceExpression, Culture),
                "System.DateTime" => GetDateTimeParseExpression(SourceExpression, Culture),
                "System.Boolean" => GetGenericParseExpression<bool>(SourceExpression),
                "System.Char" => GetGenericParseExpression<char>(SourceExpression),
                _ => throw new ArgumentException(string.Format("Conversion from {0} to {1} is not supported", "String", TargetType)),
            };
            if (Nullable.GetUnderlyingType(TargetType) == null)
            {
                return valueExp;
            }
            else
            {
                //Convert to nullable if necessary
                return Expression.Convert(valueExp, TargetType);
            }
        }
        Expression GetGenericParseExpression<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        T>(Expression sourceExpression)
        {
            MethodInfo ParseMetod = typeof(T).GetMethod("Parse", [typeof(string)])!;
            MethodCallExpression CallExpression = Expression.Call(ParseMetod, [sourceExpression]);
            return CallExpression;
        }
        Expression GetDateTimeParseExpression(Expression sourceExpression, CultureInfo culture)
        {
            MethodInfo ParseMetod = typeof(DateTime).GetMethod("Parse", [typeof(string), typeof(DateTimeFormatInfo)])!;
            ConstantExpression ProviderExpression = Expression.Constant(culture.DateTimeFormat, typeof(DateTimeFormatInfo));
            MethodCallExpression CallExpression = Expression.Call(ParseMetod, [sourceExpression, ProviderExpression]);
            return CallExpression;
        }



        MethodCallExpression GetEnumParseExpression(Expression sourceExpression, Type type)
        {
            //Get the MethodInfo for parsing an Enum
            MethodInfo EnumParseMethod = typeof(Enum).GetMethod("Parse", [typeof(Type), typeof(string), typeof(bool)])!;
            ConstantExpression TargetMemberTypeExpression = Expression.Constant(type);
            ConstantExpression IgnoreCase = Expression.Constant(true, typeof(bool));
            //Create an expression the calls the Parse method
            MethodCallExpression CallExpression = Expression.Call(EnumParseMethod, [TargetMemberTypeExpression, sourceExpression, IgnoreCase]);
            return CallExpression;
        }

//        MethodCallExpression GetNumberParseExpression(Expression sourceExpression,
//#if NET8_0_OR_GREATER
//        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
//#endif
//            Type type, CultureInfo culture)
//        {
//            MethodInfo ParseMetod = type.GetMethod("Parse", [typeof(string), typeof(NumberFormatInfo)])!;
//            ConstantExpression ProviderExpression = Expression.Constant(culture.NumberFormat, typeof(NumberFormatInfo));
//            MethodCallExpression CallExpression = Expression.Call(ParseMetod, [sourceExpression, ProviderExpression]);
//            return CallExpression;
//        }

        MethodCallExpression NumberParseExpression<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        T>(Expression source, CultureInfo culture)
        {
            MethodInfo ParseMetod = typeof(T).GetMethod("Parse", [typeof(string), typeof(NumberFormatInfo)])!;
            ConstantExpression ProviderExpression = Expression.Constant(culture.NumberFormat, typeof(NumberFormatInfo));
            MethodCallExpression CallExpression = Expression.Call(ParseMetod, [source, ProviderExpression]);
            return CallExpression;
        }
    }

    static MethodInfo changeType = typeof(Convert).GetMethod("ChangeType", [typeof(object), typeof(Type)])!;
    static MethodInfo isNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty))!;
    private static ConditionalExpression ConvertTypeExpression(Expression source, Type sourceType, Type targetType)
    {
        var underType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var isNull = Expression.Equal(source, Expression.Constant(null));
        var stringValue = Expression.Call(source, typeof(Convert).GetMethod("ToString", [sourceType])!);
        var isNullOrEmptyExpression = Expression.Call(isNullOrEmpty, stringValue);
        var canConvert = Expression.AndAlso(Expression.IsFalse(isNull), Expression.IsFalse(isNullOrEmptyExpression));
        var finalValue = Expression.Convert(Expression.Call(changeType, source, Expression.Constant(underType)), targetType);
        return Expression.Condition(canConvert, finalValue, Expression.Default(targetType));

    }
}

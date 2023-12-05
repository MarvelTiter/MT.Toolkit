using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.ExpressionHelper
{
    public class DataTypeConvert
    {
        public static Expression GetConversionExpression(Expression source, Type sourceType, Type targetType)
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
                return Expression.Call(source, sourceType.GetMethod("ToString", Type.EmptyTypes)!);
            }
            else if (ReferenceEquals(targetType, typeof(bool)))
            {
                MethodInfo ToBooleanMethod = typeof(Convert).GetMethod("ToBoolean", new[] { sourceType });
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
            Expression TargetExpression = default;
            if (ReferenceEquals(targetType, typeof(byte[])))
            {
                TargetExpression = sourceExpression;
            }
            else if (ReferenceEquals(targetType, typeof(MemoryStream)))
            {
                ConstructorInfo ConstructorInfo = targetType.GetConstructor(new[] { typeof(byte[]) });
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
                Expression ParseExpression = default;
                switch (UnderlyingType.FullName)
                {
                    case "System.Byte":
                    case "System.UInt16":
                    case "System.UInt32":
                    case "System.UInt64":
                    case "System.SByte":
                    case "System.Int16":
                    case "System.Int32":
                    case "System.Int64":
                    case "System.Double":
                    case "System.Decimal":
                        ParseExpression = GetNumberParseExpression(SourceExpression, UnderlyingType, Culture);
                        break;
                    case "System.DateTime":
                        ParseExpression = GetDateTimeParseExpression(SourceExpression, UnderlyingType, Culture);
                        break;
                    case "System.Boolean":
                    case "System.Char":
                        ParseExpression = GetGenericParseExpression(SourceExpression, UnderlyingType);
                        break;
                    default:
                        throw new ArgumentException(string.Format("Conversion from {0} to {1} is not supported", "String", TargetType));
                }
                if (Nullable.GetUnderlyingType(TargetType) == null)
                {
                    return ParseExpression;
                }
                else
                {
                    //Convert to nullable if necessary
                    return Expression.Convert(ParseExpression, TargetType);
                }
            }
            Expression GetGenericParseExpression(Expression sourceExpression, Type type)
            {
                MethodInfo ParseMetod = type.GetMethod("Parse", new[] { typeof(string) });
                MethodCallExpression CallExpression = Expression.Call(ParseMetod, new[] { sourceExpression });
                return CallExpression;
            }
            Expression GetDateTimeParseExpression(Expression sourceExpression, Type type, CultureInfo culture)
            {
                MethodInfo ParseMetod = type.GetMethod("Parse", new[] { typeof(string), typeof(DateTimeFormatInfo) });
                ConstantExpression ProviderExpression = Expression.Constant(culture.DateTimeFormat, typeof(DateTimeFormatInfo));
                MethodCallExpression CallExpression = Expression.Call(ParseMetod, new[] { sourceExpression, ProviderExpression });
                return CallExpression;
            }

            MethodCallExpression GetEnumParseExpression(Expression sourceExpression, Type type)
            {
                //Get the MethodInfo for parsing an Enum
                MethodInfo EnumParseMethod = typeof(Enum).GetMethod("Parse", new[] { typeof(Type), typeof(string), typeof(bool) });
                ConstantExpression TargetMemberTypeExpression = Expression.Constant(type);
                ConstantExpression IgnoreCase = Expression.Constant(true, typeof(bool));
                //Create an expression the calls the Parse method
                MethodCallExpression CallExpression = Expression.Call(EnumParseMethod, new[] { TargetMemberTypeExpression, sourceExpression, IgnoreCase });
                return CallExpression;
            }

            MethodCallExpression GetNumberParseExpression(Expression sourceExpression, Type type, CultureInfo culture)
            {
                MethodInfo ParseMetod = type.GetMethod("Parse", new[] { typeof(string), typeof(NumberFormatInfo) });
                ConstantExpression ProviderExpression = Expression.Constant(culture.NumberFormat, typeof(NumberFormatInfo));
                MethodCallExpression CallExpression = Expression.Call(ParseMetod, new[] { sourceExpression, ProviderExpression });
                return CallExpression;
            }
        }

        static MethodInfo changeType = typeof(Convert).GetMethod("ChangeType", [typeof(object), typeof(Type)])!;
        static MethodInfo isNullOrEmpty = typeof(string).GetMethod(nameof(string.IsNullOrEmpty))!;
        private static Expression ConvertTypeExpression(Expression source, Type sourceType, Type targetType)
        {
            var underType = Nullable.GetUnderlyingType(targetType) ?? targetType;
            var isNull = Expression.Equal(source, Expression.Constant(null));
            var stringValue = Expression.Call(source, sourceType.GetMethod("ToString", Type.EmptyTypes)!);
            var isNullOrEmptyExpression = Expression.Call(isNullOrEmpty, stringValue);
            var canConvert = Expression.AndAlso(Expression.IsFalse(isNull), Expression.IsFalse(isNullOrEmptyExpression));
            var finalValue = Expression.Convert(Expression.Call(changeType, source, Expression.Constant(underType)), targetType);
            return Expression.Condition(canConvert, finalValue, Expression.Default(targetType));

        }
    }
}

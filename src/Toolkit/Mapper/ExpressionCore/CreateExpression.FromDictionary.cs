using MT.Toolkit.ExpressionHelper;
using MT.Toolkit.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MT.KitTools.Mapper.ExpressionCore
{
    internal partial class CreateExpression
    {
        internal static void MapFromDictionary(MapInfo p, List<Expression> body)
        {
            var genericArgs = p.SourceType.GetGenericArguments();
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];
            if (keyType != typeof(string))
            {
                throw new ArgumentException("key type must be string");
            }
            if (p.TargetExpression == null)
            {
                p.TargetExpression = Expression.Variable(p.TargetType, "tar");
                body.Add(Expression.Assign(p.TargetExpression, Expression.New(p.TargetType)));
                p.Variables.Add(p.TargetExpression as ParameterExpression);
            }
            // 索引器 => dic[key]
            var getItem = p.SourceType.GetMethod("get_Item")!;
            var contain = p.SourceType.GetMethod("ContainsKey")!;
            var props = p.TargetType.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite) continue;
                var name = prop.Name;
                /*
                 * if (!source.ContainsKey(name)
                 * {
                 *     tar.XXX = ConvertType(dic[key]);
                 * }
                 */
                var assign = Expression.IfThen(
                     Expression.Call(p.SourceExpression, contain, Expression.Constant(name))
                     , Expression.Call(p.TargetExpression, prop.SetMethod!, ConvertType(Expression.Call(p.SourceExpression, getItem, Expression.Constant(name)), prop.PropertyType)
                     ));
                body.Add(assign);
            }
            if (p.ActionType == ActionType.NewObj)
                body.Add(Expression.Convert(p.TargetExpression, p.TargetType));
        }

        private static Expression ConvertType(Expression valueExpression, Type targetType)
        {
            return DataTypeConvert.GetConversionExpression(valueExpression, typeof(object), targetType);
        }
    }
}

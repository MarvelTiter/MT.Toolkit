using MT.Toolkit.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MT.KitTools.Mapper.ExpressionCore
{
	internal partial class CreateExpression
    {
        internal static void MapToDictionary(MapInfo p, List<Expression> body)
        {
            var genericArgs = p.TargetType.GetGenericArguments();
            var keyType = genericArgs[0];
            var valueType = genericArgs[1];
            if (keyType != typeof(string))
            {
                throw new ArgumentException("key type must be string");
            }
            var dicType = typeof(Dictionary<,>).MakeGenericType(genericArgs);
            MethodInfo addMethod = dicType.GetMethod("Add", genericArgs)!;
            var props = p.SourceType.GetProperties();

            // var dic = new Dictionary<string, object>();
            //ParameterExpression dicExpression = Expression.Variable(dicType, "dic");
            //body.Add(dicExpression);
            //body.Add(Expression.Assign(dicExpression, Expression.New(dicType)));
            // dic.Add();
            if (p.TargetExpression == null)
            {
                p.TargetExpression = Expression.Variable(dicType, "dic");
                body.Add(Expression.Assign(p.TargetExpression, Expression.New(dicType)));
                p.Variables.Add(p.TargetExpression as ParameterExpression);
            }
            foreach (PropertyInfo property in props)
            {
                if (!property.CanRead) continue;
                if (valueType == typeof(object) || valueType == property.PropertyType)
                {
                    var key = Expression.Constant(property.Name, keyType);
                    var value = Expression.Property(p.SourceExpression, property);
                    MethodCallExpression callAdd = Expression.Call(p.TargetExpression, addMethod, key, Expression.Convert(value, valueType));
                    body.Add(callAdd);
                }
            }
            // Func 需要 return dic;
            if (p.ActionType == ActionType.NewObj)
                body.Add(Expression.Convert(p.TargetExpression, p.TargetType));
        }
    }
}

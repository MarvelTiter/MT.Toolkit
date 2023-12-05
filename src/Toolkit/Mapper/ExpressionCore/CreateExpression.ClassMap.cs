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
        internal static void ClassMap(MapInfo p, List<Expression> body)
        {
            Type targetType = p.TargetType;
            var rules = p.MapRule.Maps;
            if (p.TargetExpression == null)
            {
                p.TargetExpression = Expression.Variable(targetType, "tar");
                body.Add(Expression.Assign(p.TargetExpression, Expression.New(targetType)));
                p.Variables.Add(p.TargetExpression as ParameterExpression);
            }

            foreach (var rule in rules)
            {
                Expression valueExp = GetValueExpression(p.SourceExpression, rule);
                MethodCallExpression setPropExp = Expression.Call(p.TargetExpression, rule.MapTo.SetMethod, valueExp);
                body.Add(setPropExp);
            }

            if (p.MapRule.MapPostAction != null)
            {
                var dyInvoke = typeof(Delegate).GetMethod("DynamicInvoke", new[] { typeof(object[]) })!;
                var actionExp = Expression.Constant(p.MapRule.MapPostAction, typeof(Delegate));

                MethodCallExpression postExp = Expression.Call(actionExp, dyInvoke, Expression.NewArrayInit(typeof(object), p.SourceExpression, p.TargetExpression));
                body.Add(postExp);
            }

            if (p.ActionType == ActionType.NewObj)
                body.Add(Expression.Convert(p.TargetExpression, p.TargetType));

            //var targetProps = targetType.GetProperties();
            //var fromProps = fromType.GetProperties();
            //foreach (var tar in targetProps)
            //{
            //    if (!tar.CanWrite) continue;
            //    var from = fromProps.FirstOrDefault(f => f.Name.ToLower() == tar.Name.ToLower());
            //    if (from == null) continue;
            //    //MemberExpression tarProp = Expression.Property(targetExp, tar);
            //    MemberExpression fromProp = Expression.Property(p.SourceExpression, from);
            //    var converted = DataTypeConvert.GetConversionExpression(fromProp, tar.PropertyType, from.PropertyType);
            //    MethodCallExpression setPropExp = Expression.Call(p.TargetExpression, tar.SetMethod, converted);
            //    body.Add(setPropExp);
            //}
            //var source = p.SourceExpression as ParameterExpression;
            //List<MemberBinding> bindings = new List<MemberBinding>();
            //initBindings(ref bindings, source, p.Rules);
            //MemberInitExpression init = Expression.MemberInit(Expression.New(p.TargetElementType), bindings);
            //body.Add(init);
        }

        internal static MemberInitExpression ClassMap(List<MappingInfo> rules, Type sourceType, Type targetType, object source)
        {
            ParameterExpression parameterExpression = Expression.Variable(sourceType, "source");
            Expression.Assign(parameterExpression, Expression.Constant(source, sourceType));
            List<MemberBinding> bindings = new List<MemberBinding>();
            initBindings(ref bindings, parameterExpression, rules);
            return Expression.MemberInit(Expression.New(targetType), bindings);
        }

        private static void initBindings(ref List<MemberBinding> memberBindings, ParameterExpression parameterExpression, IList<MappingInfo> rules)
        {
            foreach (var rule in rules)
            {
                Expression valueExp = GetValueExpression(parameterExpression, rule);
                MemberAssignment bind = Expression.Bind(rule.MapTo, valueExp);
                memberBindings.Add(bind);
            }
        }
        private static Expression GetValueExpression(Expression parameter, MappingInfo rule)
        {
            var prop = rule.MapFrom;
            var bind = Expression.Property(parameter, prop);
            Expression convertedBind = DataTypeConvert.GetConversionExpression(bind, rule.MapFrom.PropertyType, rule.MapTo.PropertyType);
            return convertedBind;
        }
    }
}

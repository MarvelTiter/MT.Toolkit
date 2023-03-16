using MT.Toolkit.ExpressionHelper;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace MT.Toolkit.DataTableExtension
{
	internal class TableExpressionBase
	{
		public static MethodInfo Datarow_getItem = typeof(DataRow).GetMethod("get_Item", new Type[] { typeof(string) });
		public static MethodInfo DataRow_IsNull = typeof(DataRow).GetMethod("IsNull", new Type[] { typeof(string) });
		public static Expression GetTargetValueExpression(DataColumn column, ParameterExpression parameterExpression, Type targetType)
		{
			MethodCallExpression rowObjExp = Expression.Call(parameterExpression, Datarow_getItem, Expression.Constant(column.ColumnName));
			MethodCallExpression checkNullExp = Expression.Call(parameterExpression, DataRow_IsNull, Expression.Constant(column.ColumnName));
			Expression e;
			Type t = column.DataType;
			if (targetType != typeof(byte[]))
			{
				e = Expression.Call(rowObjExp, typeof(object).GetMethod("ToString", Type.EmptyTypes));
				t = typeof(string);
			}
			else
				e = rowObjExp;
			Expression realValueExp = DataTypeConvert.GetConversionExpression(e, t, targetType);
			if (column.AllowDBNull)
			{
				return Expression.Condition(
					checkNullExp,
					Expression.Default(targetType),
					realValueExp,
					targetType
					);
			}
			else
			{
				return realValueExp;
			}
		}
	}
}

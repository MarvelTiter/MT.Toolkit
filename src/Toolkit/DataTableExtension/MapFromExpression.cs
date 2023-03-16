using MT.Toolkit.DataTableExtension;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MT.Toolkit.DataTableExtension
{
	public static class MapFromExpression<T>
	{
		static Dictionary<string, Action<T, DataRow>> actions = new Dictionary<string, Action<T, DataRow>>();
		public static Action<T, DataRow> Build(DataColumnCollection cols)
		{
			var columnNames = from col in cols.Cast<DataColumn>()
							  select col.ColumnName;
			var key = string.Join("_", columnNames);
			if (actions.TryGetValue(key, out var action))
			{
				return action;
			}
			action = CreateAction(cols);
			actions.Add(key, action);
			return action;
		}

		private static Action<T, DataRow> CreateAction(DataColumnCollection cols)
		{
			ParameterExpression rowExp = Expression.Parameter(typeof(DataRow), "row");
			ParameterExpression tarExp = Expression.Parameter(typeof(T), "tar");
			var tarType = typeof(T);
			List<Expression> body = new List<Expression>();
			// properties
			var props = tarType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			foreach (DataColumn col in cols)
			{
				var prop = props.FirstOrDefault(p => p.Name.ToLower() == col.ColumnName.ToLower());
				if (prop != null && prop.CanWrite)
				{
					var valueExp = TableExpressionBase.GetTargetValueExpression(col, rowExp, prop.PropertyType);
					MethodCallExpression propAssign = Expression.Call(tarExp, prop.SetMethod, valueExp);
					body.Add(propAssign);
				}
			}
			var block = Expression.Block(body);
			var lambda = Expression.Lambda<Action<T, DataRow>>(block, tarExp, rowExp);
			return lambda.Compile();
		}

	}
}

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

    internal static class ExpressionUsedMethods
    {
        public static MethodInfo DataColumnAdd = typeof(DataColumnCollection).GetMethod(nameof(DataColumnCollection.Add), [typeof(string), typeof(Type)])!;
        public static MethodInfo DataRowSet = typeof(DataRow).GetMethod("set_Item", [typeof(string), typeof(object)])!;
    }

    internal static class MapToDataTableExtension<T>
    {
        private static Func<DataTable> createTable;
        private static Action<T, DataRow> fillRow;

        static MapToDataTableExtension()
        {
            createTable = BuildCreateTableFunc();
            fillRow = BuildCreateRowFunc();
        }

        internal static DataTable GetDataTable() => createTable.Invoke();
        internal static void FillDataRow(T value, DataRow row) => fillRow.Invoke(value, row);
        private static Func<DataTable> BuildCreateTableFunc()
        {
            var dtExp = Expression.Variable(typeof(DataTable));
            var ctorMethod = typeof(DataTable).GetConstructor([typeof(string)])!;
            var newDt = Expression.New(ctorMethod, [Expression.Constant(typeof(T).Name, typeof(string))]);
            var assign = Expression.Assign(dtExp, newDt);

            List<Expression> body = [
                dtExp,assign
                ];
            var props = typeof(T).GetProperties();
            var columns = Expression.Property(dtExp, nameof(DataTable.Columns));
            foreach (var item in props)
            {
                if (!item.CanRead) continue;
                body.Add(Expression.Call(columns, ExpressionUsedMethods.DataColumnAdd, Expression.Constant(item.Name, typeof(string)), Expression.Constant(item.PropertyType, typeof(Type))));
            }
            body.Add(dtExp);
            var block = Expression.Block([dtExp], [.. body]);
            var lambda = Expression.Lambda<Func<DataTable>>(block);
            return lambda.Compile();
        }

        private static Action<T, DataRow> BuildCreateRowFunc()
        {
            var val = Expression.Parameter(typeof(T), "p");
            var row = Expression.Parameter(typeof(DataRow), "r");
            var props = typeof(T).GetProperties();
            List<Expression> body = [];
            foreach (var item in props)
            {
                if (!item.CanRead) continue;
                body.Add(Expression.Call(row, ExpressionUsedMethods.DataRowSet, Expression.Constant(item.Name, typeof(string)), Expression.Convert(Expression.Property(val, item), typeof(object))));
            }
            var block = Expression.Block( [.. body]);
            var lambda = Expression.Lambda<Action<T, DataRow>>(block, val, row);
            return lambda.Compile();
        }
    }
}

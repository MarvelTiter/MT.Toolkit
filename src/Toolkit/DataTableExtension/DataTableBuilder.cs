using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace MT.Toolkit.DataTableExtension;

internal static class DataTableBuilder<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
T>
{
    private static Func<DataRow, object>? func;

    public static Func<DataRow, object> Build(DataColumnCollection dataColumn, bool mapAll)
    {
        if (func != null) return func;
        func = CreateFunc(dataColumn, mapAll);
        return func;
    }

    static Func<DataRow, object> CreateFunc(DataColumnCollection cols, bool mapAll)
    {
        Type tarType = typeof(T);
        // Datarow row; 
        ParameterExpression rowExp = Expression.Parameter(typeof(DataRow), "row");
        List<MemberBinding> bindings = [];
        // fields
        var fields = tarType.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            void work()
            {
                if (cols.Contains(field.Name))
                {
                    DataColumn col = cols[field.Name]!;
                    var valueExp = TableExpressionBase.GetTargetValueExpression(col, rowExp, field.FieldType);
                    MemberAssignment memberAssignment = Expression.Bind(field, valueExp);
                    bindings.Add(memberAssignment);
                    return;
                }
                if (mapAll)
                {
                    throw new ArgumentException($"Property {field.Name} is not matched by any column in datatable");
                }
            }
            work();
        }

        // properties
        var props = tarType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in props)
        {
            void work()
            {
                if (cols.Contains(prop.Name))
                {
                    if (!prop.CanWrite) return;
                    DataColumn col = cols[prop.Name]!;
                    var valueExp = TableExpressionBase.GetTargetValueExpression(col, rowExp, prop.PropertyType);
                    MemberAssignment memberAssignment = Expression.Bind(prop, valueExp);
                    bindings.Add(memberAssignment);
                    return;
                }
                if (mapAll)
                {
                    throw new ArgumentException($"Property {prop.Name} is not matched by any column in datatable");
                }

            }
            work();
        }
        MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(tarType), bindings);
        var lambda = Expression.Lambda<Func<DataRow, object>>(memberInitExpression, rowExp);
        return lambda.Compile();
    }

}

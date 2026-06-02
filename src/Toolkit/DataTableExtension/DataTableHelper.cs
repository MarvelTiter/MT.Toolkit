using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace MT.Toolkit.DataTableExtension;

/// <summary>
/// DataTable帮助类
/// </summary>
public static class DataTableHelper
{
    /// <summary>
    /// 是否有至少一行数据
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static bool HasRows(this DataTable? dt)
    {
        return dt is not null && dt.Rows.Count > 0;
    }

    /// <summary>
    /// 将DataTable转换为可枚举的对象集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="mapAllFields"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToEnumerable<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this DataTable? self, bool mapAllFields = false)
    {
        if (self is null)
        {
            yield break;
        }
        foreach (DataRow row in self.Rows)
        {
            yield return row.Parse<T>(mapAllFields);
        }
    }

    /// <summary>
    /// 将DataTable转换为可枚举的对象集合，并根据指定的过滤条件进行筛选
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="filter"></param>
    /// <param name="mapAllFields"></param>
    /// <returns></returns>
    public static IEnumerable<T> Select<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this DataTable self, Func<DataRow, bool> filter, bool mapAllFields = false)
    {
        foreach (DataRow row in self.Rows)
        {
            if (filter.Invoke(row))
            {
                yield return row.Parse<T>(mapAllFields);
            }
        }
    }

    /// <summary>
    /// 将DataTable转换为可枚举的对象集合，并根据指定的过滤条件进行筛选
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="filter"></param>
    /// <param name="mapAllFields"></param>
    /// <returns></returns>
    public static IEnumerable<T> Select<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this DataTable self, Func<T, bool> filter, bool mapAllFields = false)
    {
        foreach (DataRow row in self.Rows)
        {
            var t = row.Parse<T>(mapAllFields);
            if (filter.Invoke(t))
            {
                yield return t;
            }
        }
    }

    /// <summary>
    /// 转换DataRow为对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="row"></param>
    /// <param name="mapAllFields"></param>
    /// <returns></returns>
    public static T Parse<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this DataRow row, bool mapAllFields)
    {
        var columns = row.Table.Columns;
        var func = DataTableBuilder<T>.Build(columns, mapAllFields);
        return (T)func.Invoke(row);
    }

    /// <summary>
    /// 获取DataRow中指定列的值，并将其转换为指定类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static T GetValue<T>(this DataRow self, string key)
    {
        if (!self.Table.Columns.Contains(key))
        {
            throw new ArgumentException($"column {key} is not contains in datatable");
        }

        if (self.IsNull(key))
        {
            return default!;
        }
        var val = self[key];

        var undeylying = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        return (T)Convert.ChangeType(val, undeylying);
    }

    /// <summary>
    /// 获取DataRow中指定列的值, 并将其转换为字符串
    /// </summary>
    /// <param name="self"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static string? GetValue(this DataRow self, string key)
    {
        if (!self.Table.Columns.Contains(key))
        {
            throw new ArgumentException($"column {key} is not contains in datatable");
        }

        if (self.IsNull(key))
        {
            return default;
        }
        var val = self[key];
        return val?.ToString();
    }

    /// <summary>
    /// 从DataTable的第一行数据映射到对象实例中
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="source"></param>
    public static void MapFromTable<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this T self, DataTable? source)
    {
        if (source is null) 
            return;
        var action = MapFromExpression<T>.Build(source.Columns);
        action?.Invoke(self, source.Rows[0]);
    }

    /// <summary>
    /// 获取DataTable的第一行数据
    /// </summary>
    /// <param name="dt"></param>
    /// <returns></returns>
    public static DataRow? FirstRow(this DataTable? dt)
    {
        if (dt?.Rows.Count == 0)
        {
            return null;
        }
        return dt?.Rows[0];
    }

    /// <summary>
    /// 实体集合转换为DataTable
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="datas"></param>
    /// <returns></returns>
    public static DataTable ToDataTable<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    T>(this IEnumerable<T> datas)
    {
        var dt = MapToDataTableExtension<T>.GetDataTable();
        foreach (var item in datas)
        {
            var row = dt.NewRow();
            MapToDataTableExtension<T>.FillDataRow(item, row);
            dt.Rows.Add(row);
        }
        //dt.Columns.Add()
        return dt;
    }
}


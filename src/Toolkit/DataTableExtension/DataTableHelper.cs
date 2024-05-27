using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace MT.Toolkit.DataTableExtension
{
    public static class DataTableHelper
    {
        public static bool HasRows(this DataTable dt)
        {
            return dt != null && dt.Rows.Count > 0;
        }
        public static IEnumerable<T> ToEnumerable<T>(this DataTable self, bool mapAllFields = false)
        {
            foreach (DataRow row in self.Rows)
            {
                yield return row.Parse<T>(mapAllFields);
            }
        }

        public static IEnumerable<T> Select<T>(this DataTable self, Func<DataRow, bool> filter, bool mapAllFields = false)
        {
            foreach (DataRow row in self.Rows)
            {
                if (filter.Invoke(row))
                {
                    yield return row.Parse<T>(mapAllFields);
                }
            }
        }

        public static IEnumerable<T> Select<T>(this DataTable self, Func<T, bool> filter, bool mapAllFields = false)
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

        public static T Parse<T>(this DataRow row, bool mapAllFields)
        {
            var columns = row.Table.Columns;
            var func = DataTableBuilder<T>.Build(columns, mapAllFields);
            return (T)func.Invoke(row);
        }

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

        public static void MapFromTable<T>(this T self, DataTable source)
        {
            var action = MapFromExpression<T>.Build(source.Columns);
            action?.Invoke(self, source.Rows[0]);
        }

        public static DataRow? FirstRow(this DataTable dt)
        {
            if (dt?.Rows.Count == 0)
            {
                return null;
            }
            return dt?.Rows[0];
        }
    }
}


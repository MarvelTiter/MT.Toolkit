using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.TypeConvertHelper;

/// <summary>
/// 
/// </summary>
public static class ObjectConvertHelper
{
    /// <summary>
    /// 类型转换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <returns></returns>
    public static T? ChangeType<T>(this object self)
    {
        return (T?)ChangeType(self, typeof(T));
    }

    /// <summary>
    /// 类型转换
    /// </summary>
    /// <param name="self"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static object? ChangeType(this object self, Type type)
    {
        if (self is null)
        {
            return default;
        }
        type = Nullable.GetUnderlyingType(type) ?? type;
        return Convert.ChangeType(self, type);
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.TypeExtensions;

/// <summary>
/// 类型扩展方法
/// </summary>
public static class TypeExtensions
{
    private static readonly string System_Collections_Generic_Dictionary = "System.Collections.Generic.Dictionary";
    private static readonly string System_Collections_Generic_IDictionary = "System.Collections.Generic.IDictionary";
    private static readonly string System_Collections_Generic_ICollection_1 = "System.Collections.Generic.ICollection`1";
    private static readonly string System_Collections_Generic_IEnumerable_1 = "System.Collections.Generic.IEnumerable`1";

    /// <summary>
    /// 是否是字典类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsDictionary(
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
    this Type type)
    {
        var interfaces = type.GetInterfaces();
        return type.FullName?.StartsWith(System_Collections_Generic_Dictionary) == true ||
            type.FullName?.StartsWith(System_Collections_Generic_IDictionary) == true ||
            type.GetInterfaces().Any(tp => tp.FullName?.StartsWith(System_Collections_Generic_IDictionary) == true);
    }

    /// <summary>
    /// 是否是可枚举类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsIEnumerableType(
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        this Type type)
    {
        return type.FullName?.StartsWith(System_Collections_Generic_IEnumerable_1) == true ||
            type.GetInterfaces().Any(tp => tp.FullName?.StartsWith(System_Collections_Generic_IEnumerable_1) == true);
    }

    /// <summary>
    /// 是否是集合类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsICollectionType(
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        this Type type)
    {
        return type.FullName?.StartsWith(System_Collections_Generic_ICollection_1) == true ||
            type.GetInterfaces().Any(tp => tp.FullName?.StartsWith(System_Collections_Generic_ICollection_1) == true);
    }

    /// <summary>
    /// 是否是可空类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsNullableType(this Type type)
    {
        return type.FullName?.StartsWith("System.Nullable`1[") == true;
    }

    /// <summary>
    /// 获取集合元素类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type GetCollectionElementType(
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]
#endif
        this Type type)
    {
        if (type.IsArray) { type.GetElementType(); }
        if (type.IsGenericEnumerableType()) { return type.GetGenericArguments()[0]; }
        var arrayType = Array.Find(type.GetInterfaces(), IsGenericEnumerableType);
        if (arrayType == null) { return typeof(object); }
        return arrayType.GetGenericArguments()[0];
    }

    /// <summary>
    /// 是否是泛型枚举类型
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private static bool IsGenericEnumerableType(this Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
    }
}

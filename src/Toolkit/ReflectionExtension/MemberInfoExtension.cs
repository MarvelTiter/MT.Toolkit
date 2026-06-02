using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace MT.Toolkit.ReflectionExtension;

/// <summary>
/// 
/// </summary>
public static class MemberInfoExtension
{
    /// <summary>
    /// 调用方法
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
    [UnconditionalSuppressMessage("AOT", "IL2075:Calling members from object.GetType() may break functionality when AOT compiling.", Justification = "GetMethod")]
#endif
    public static void Invoke(this object obj, string methodName, params object[] args)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method == null) return;
        var parameter = Expression.Parameter(type, "e");
        var callExpression = Expression.Call(parameter, method, args.Select(Expression.Constant));
        Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj);
    }

    /// <summary>
    /// 调用方法（泛型）
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
#endif
    public static void Invoke<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
    T>(this T obj, string methodName, params object[] args)
    {
        var type = typeof(T);
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method == null) return;
        var parameter = Expression.Parameter(type, "e");
        var callExpression = Expression.Call(parameter, method, args.Select(Expression.Constant));
        Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj);
    }

    /// <summary>
    /// 调用方法（有返回值）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
    [UnconditionalSuppressMessage("AOT", "IL2075:Calling members from object.GetType() may break functionality when AOT compiling.", Justification = "GetMethod")]
#endif
    public static T Invoke<T>(this object obj, string methodName, params object[] args)
    {
        var type = obj.GetType();
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method == null) return default!;
        var parameter = Expression.Parameter(type, "e");
        var callExpression = Expression.Call(parameter, method, args.Select(Expression.Constant));
        return (T)Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj)!;
    }

    /// <summary>
    /// 调用方法（有返回值）
    /// </summary>
    /// <typeparam name="TObj"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
#endif
    public static T Invoke<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
    TObj, T>(this TObj obj, string methodName, params object[] args)
    {
        var type = typeof(TObj);
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method == null) return default!;
        var parameter = Expression.Parameter(type, "e");
        var callExpression = Expression.Call(parameter, method, args.Select(Expression.Constant));
        return (T)Expression.Lambda(callExpression, parameter).Compile().DynamicInvoke(obj)!;
    }

    /// <summary>
    /// 根据类型调用静态方法
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
#endif
    public static void Invoke(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        this Type type, string methodName, params object[] args)
    {
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method is null) return;
        MethodCallExpression methodCallExpression = Expression.Call(method, args.Select(Expression.Constant));
        Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
    }

    /// <summary>
    /// 根据类型调用静态方法(有返回值)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
#endif
    public static T Invoke<T>(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        this Type type, string methodName, params object[] args)
    {
        var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
        if (method == null)
            return default!;
        MethodCallExpression methodCallExpression = Expression.Call(method, args.Select(Expression.Constant));
        return (T)Expression.Lambda(methodCallExpression).Compile().DynamicInvoke()!;
    }

    /// <summary>
    /// 反射调用指定方法（有返回值）
    /// </summary>
    /// <param name="type"></param>
    /// <param name="genericType"></param>
    /// <param name="methodName"></param>
    /// <param name="args"></param>
    /// <returns></returns>
#if NET8_0_OR_GREATER
    [UnconditionalSuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Expression.Lambda")]
    [UnconditionalSuppressMessage("AOT", "IL2060:Calling MakeGenericMethod may break functionality when AOT compiling.", Justification = "")]
#endif
    public static object Invoke(
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        this Type type, Type genericType, string methodName, params object[] args)
    {
        var method = type.GetMethod(methodName, [.. args.Select(o => o.GetType())]);
        if (method == null) return default!;
        method = method.MakeGenericMethod(genericType);
        MethodCallExpression methodCallExpression = Expression.Call(method, args.Select(Expression.Constant));
        return Expression.Lambda(methodCallExpression).Compile().DynamicInvoke()!;
    }

    /// <summary>
    /// 创建获取属性值的委托
    /// </summary>
    /// <typeparam name="TProp"></typeparam>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static Func<object, TProp> GetPropertyAccessor<TProp>(this PropertyInfo prop)
    {
        /*
         * p => (object)p.XXX;
         */
        if (prop.DeclaringType == null || !prop.CanRead) return NullGetter<TProp>;
        var p = Expression.Parameter(typeof(object), "p");
        var instance = Expression.Convert(p, prop.DeclaringType);
        var propExp = Expression.Property(instance, prop);
        Expression body = propExp;
        if (typeof(TProp) == typeof(object))
        {
            body = Expression.Convert(propExp, typeof(object));
        }
        var lambda = Expression.Lambda<Func<object, TProp>>(body, p);
        return lambda.Compile();
    }
    /// <summary>
    /// 创建获取属性值的委托
    /// </summary>
    /// <returns></returns>
    public static Func<object, object> GetPropertyAccessor<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    TEntity>(this TEntity _, string propName)
    {
        var prop = typeof(TEntity).GetProperty(propName);
        if (prop == null) return NullGetter<object>;
        return prop.GetPropertyAccessor<object>();
    }

    static TProp NullGetter<TProp>(object entity) => default!;
    /// <summary>
    /// 创建属性赋值的委托
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static Action<object, object> GetPropertySetter(this PropertyInfo prop)
    {
        /*
         * (p, v) => ((T)p).XXX = (TProp)v; 
         */
        if (prop.DeclaringType == null || !prop.CanWrite) return NullSetter;
        var p = Expression.Parameter(typeof(object), "p");
        var val = Expression.Parameter(typeof(object), "v");
        var cp = Expression.Convert(p, prop.DeclaringType);
        var setMethod = prop.SetMethod!;
        var set = Expression.Call(cp, setMethod, Expression.Convert(val, prop.PropertyType));
        var lambda = Expression.Lambda<Action<object, object>>(set, p, val);
        return lambda.Compile();
    }
    /// <summary>
    /// 创建属性赋值的委托
    /// </summary>
    /// <returns></returns>
    public static Action<object, object> GetPropertySetter<
#if NET8_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
#endif
    T>(this T _, string propName)
    {
        var prop = typeof(T).GetProperty(propName);
        if (prop == null) return NullSetter;
        return prop.GetPropertySetter();
    }
    static void NullSetter(object _1, object _2) { }
}

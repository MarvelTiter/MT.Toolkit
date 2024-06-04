using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Linq;

namespace MT.Toolkit.ReflectionExtension
{
    public static class MemberInfoExtension
    {

        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        public static void Invoke(this object obj, string methodName, params object[] args)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
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
        public static T Invoke<T>(this object obj, string methodName, params object[] args)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
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
        public static void Invoke(this Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
            MethodCallExpression methodCallExpression = Expression.Call(method, args.Select(Expression.Constant));
            Expression.Lambda(methodCallExpression).Compile().DynamicInvoke();
        }

        /// <summary>
        /// 根据类型调用静态方法(有返回值)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="args"></param>
        public static T Invoke<T>(this Type type, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
            if (method == null)
                return default!;
            MethodCallExpression methodCallExpression = Expression.Call(method, args.Select(Expression.Constant));
            return (T)Expression.Lambda(methodCallExpression).Compile().DynamicInvoke()!;
        }
        public static object Invoke(this Type type, Type genericType, string methodName, params object[] args)
        {
            var method = type.GetMethod(methodName, args.Select(o => o.GetType()).ToArray());
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
        /// <typeparam name="TProp"></typeparam>
        /// <param name="entity"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static Func<object, TProp> GetPropertyAccessor<TProp>(this object entity, string propName)
        {
            var prop = entity.GetType().GetProperty(propName);
            if (prop == null) return NullGetter<TProp>;
            return prop.GetPropertyAccessor<TProp>();
        }

        static TProp NullGetter<TProp>(object  entity) => default!;
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
        /// <param name="entity"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static Action<object, object> GetPropertySetter(this object entity, string propName)
        {
            var prop = entity.GetType().GetProperty(propName);
            if (prop == null) return NullSetter;
            return prop.GetPropertySetter();
        }
        static void NullSetter(object _1, object _2) { }
    }
}

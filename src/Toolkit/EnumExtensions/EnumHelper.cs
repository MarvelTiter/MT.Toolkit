using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.EnumExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumHelper
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<MemberInfo, string>> enumCache = [];
        /// <summary>
        /// 获取枚举的显示名称
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDisplayName<
#if NET8_0_OR_GREATER
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
        T>(this T @enum) where T : Enum
        {
            var et = typeof(T);
            var member = et.GetMember(@enum.ToString()).First();
            if (!enumCache.TryGetValue(et, out var edic))
            {
                edic = new();
                enumCache.TryAdd(et, edic);
            }
            if (!edic.TryGetValue(member, out var des))
            {
                if (member.GetCustomAttribute<DisplayAttribute>() is { } displayAttr)
                {
                    des = displayAttr.Name;
                }
                else if (member.GetCustomAttribute<DescriptionAttribute>() is { } descriptionAttr)
                {
                    des = descriptionAttr.Description;
                }
                des ??= @enum.ToString();
                edic.TryAdd(member, des);
            }
            return des;
        }

    }
}

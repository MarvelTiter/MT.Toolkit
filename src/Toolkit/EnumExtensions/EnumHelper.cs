using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private static readonly ConcurrentDictionary<(Type, string), string> enumCache = [];
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enum"></param>
        /// <returns></returns>
        public static string GetDisplayName<T>(this T @enum) where T : Enum
        {
            return enumCache.GetOrAdd((typeof(T), @enum.ToString()), key =>
            {
                var member = key.Item1.GetMember(key.Item2).FirstOrDefault();
                if (member != null)
                {
                    var displayAttr = member.GetCustomAttribute<DisplayAttribute>();
                    if (displayAttr is not null)
                    {
                        return displayAttr.Name ?? key.Item2;
                    }
                }
                return key.Item2;
            });
        }

    }
}

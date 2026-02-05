using System;

namespace MT.Toolkit.Mapper
{
    [Obsolete("使用AutoGenMapperGenerator代替")]
    public static partial class MapperExtensions
    {
        /// <summary>
        /// 为现有的对象赋值
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <typeparam name="TFrom"></typeparam>
        /// <param name="self"></param>
        /// <param name="source"></param>
        public static void Map<TFrom, TTarget>(this TTarget self, TFrom source)
        {
            InnerMapper<TFrom, TTarget>.Invoke(source, self);
        }


        /// <summary>
        /// 泛型缓存
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        internal static class MapperLink<TFrom, TTarget>
        {
            private static readonly Func<object, TTarget> converter;
            private static Profiles profile = null;
            static MapperLink()
            {
                profile = new MappingProfile<TFrom, TTarget>();
                converter = (Func<object, TTarget>)profile.CreateDelegate(ActionType.NewObj);
            }

            public static TTarget Create(TFrom source)
            {
                return converter.Invoke(source);
            }
        }

        internal static class InnerMapper<TFrom, TTarget>
        {
            private static readonly Action<TFrom, TTarget> action;
            private static Profiles profile = null;

            static InnerMapper()
            {
                profile = new MappingProfile<TFrom, TTarget>();
                action = (Action<TFrom, TTarget>)profile.CreateDelegate(ActionType.Ref);
            }

            public static void Invoke(TFrom source, TTarget target)
            {
                action?.Invoke(source, target);
            }
        }
    }
}

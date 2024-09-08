using System;
using System.Collections.Generic;

namespace MT.Toolkit.Mapper
{
    public interface ISimpleMapper
    {
        TTarget Map<TFrom, TTarget>(TFrom source);
        IEnumerable<TTarget> Map<TFrom, TTarget>(IEnumerable<TFrom> sources);
        MapperConfig Configuration { get; }
    }
    
    [Obsolete("使用AutoGenMapperGenerator.AutoMapperGenerator")]
    public class SimpleMapper : ISimpleMapper
    {
        private readonly Mapper _map;
        public SimpleMapper()
        {
            _map = Mapper.Default;
        }

        public MapperConfig Configuration => _map.Config;

        public TTarget Map<TFrom, TTarget>(TFrom source)
        {
            return _map.NewMap<TFrom, TTarget>(source);
        }

        public IEnumerable<TTarget> Map<TFrom, TTarget>(IEnumerable<TFrom> sources)
        {
            foreach (var item in sources)
            {
                yield return Map<TFrom, TTarget>(item);
            }
        }
    }

    [Obsolete("使用AutoGenMapperGenerator.AutoMapperGenerator")]
    public class Mapper
    {
        public static TValue Map<TValue>(TValue source)
        {
            return Default.NewMap<TValue, TValue>(source);
        }
        public static TTarget Map<TFrom, TTarget>(TFrom source)
        {
            return Default.NewMap<TFrom, TTarget>(source);
        }
        public static IEnumerable<TTarget> Map<TFrom, TTarget>(IEnumerable<TFrom> sources)
        {
            foreach (var item in sources)
            {
                yield return Map<TFrom, TTarget>(item);
            }
        }
        /// <summary>
        /// 创建 MappingProfile，并检查是否重复
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <returns></returns>
        //internal static MapperRule<TFrom, TTarget> CreateMappingRule<TFrom, TTarget>()
        //{
        //    var rule = new MapperRule<TFrom, TTarget>();
        //    MapRuleProvider.Cache(rule, typeof(TFrom), typeof(TTarget));
        //    return rule;
        //}
        public static Mapper Default => new Mapper();

        internal MapperConfig Config => MapperConfigProvider.GetMapperConfig();
        internal Mapper() { }

        public Mapper Configuration(Action<MapperConfig> config)
        {
            config?.Invoke(Config);
            return this;
        }

        /// <summary>
        /// 配置映射
        /// </summary>
        /// <typeparam name="TFrom"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="context"></param>
        [Obsolete("使用AutoGenMapperGenerator.AutoMapperGenerator")]
        public Mapper CreateMap<TFrom, TTarget>(Action<MapperRule<TFrom, TTarget>> context = null)
        {
            var map = (MapperRule<TFrom, TTarget>)MapRuleProvider.GetMapRule<TFrom, TTarget>();
            context?.Invoke(map);
            return this;
        }
        public TTarget NewMap<TFrom, TTarget>(TFrom source)
        {
            return MapperExtensions.MapperLink<TFrom, TTarget>.Create(source);
        }
    }
}

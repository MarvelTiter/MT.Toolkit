using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MT.Toolkit.Mapper
{
	public static class MapRuleProvider
	{
		private static IList<IMapperRule> cache = new List<IMapperRule>();

		public static void Cache(IMapperRule profiles, Type sourceType, Type targetType)
		{
			bool contain = cache.Any(p => p.Equal(sourceType, targetType));
			if (!contain)
			{
				cache.Add(profiles);
				//throw new ArgumentException($"mapping between {sourceType.Name} and {targetType.Name} had been created");
			}
		}

		public static IMapperRule GetMapRule<TSource, TTarget>()
		{
			return GetMapRule(typeof(TSource), typeof(TTarget));
		}

		public static IMapperRule GetMapRule(Type sourceType, Type targetType)
		{
			var profile = cache.FirstOrDefault(p => p.Equal(sourceType, targetType));
			if (profile == null)
			{
				profile = CreateMapRule(sourceType, targetType);
				cache.Add(profile);
			}
			return profile;
		}

		public static IMapperRule CreateMapRule(Type sourceType, Type targetType)
		{
			Type genericType = typeof(MapperRule<,>).MakeGenericType(sourceType, targetType);
			var exp = Expression.New(genericType);
			return (IMapperRule)Expression.Lambda(exp).Compile().DynamicInvoke();
		}
	}
}

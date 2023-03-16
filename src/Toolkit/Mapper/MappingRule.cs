using MT.Toolkit.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MT.Toolkit.Mapper
{
	public static class TypeExtensions
	{
		public static bool Equal(this IMapperRule self, Type source, Type target)
		{
			return ReferenceEquals(self.SourceType, source) && ReferenceEquals(self.TargetType, target);
		}
	}
	public interface IMapperRule
	{
		List<MappingInfo> Maps { get; set; }
		Delegate MapPostAction { get; set; }
		Type SourceType { get; set; }
		Type TargetType { get; set; }
	}

	public class MapperRule<TSource, TTarget> : IMapperRule
	{
		public Type SourceType { get; set; }
		public Type TargetType { get; set; }
		public Delegate MapPostAction { get; set; }
		public List<MappingInfo> Maps { get; set; }
		private MapperConfig Config => MapperConfigProvider.GetMapperConfig();
		public MapperRule()
		{
			SourceType = typeof(TSource);
			TargetType = typeof(TTarget);
			Maps = new List<MappingInfo>();
			AutoMap();
		}

		public void Mapping(Action<TSource, TTarget> action)
		{
			MapPostAction = action;
		}

		public void AutoMap()
		{
			if (SourceType.IsDictionary() || TargetType.IsDictionary())
			{
				return;
			}
			Maps.Clear();
			var targetProps = TargetType.GetProperties();
			var sourceProps = SourceType.GetProperties();
			foreach (var tar in targetProps)
			{
				var source = sourceProps.FirstOrDefault(p => Config.Match(p, tar));
				if (tar.CanWrite && !Maps.Any(r => r.MapTo == tar) && source != null)
					AddMap(tar, source);
			}
		}
		private void AddMap(PropertyInfo to, PropertyInfo from, string formatter = null)
		{
			MappingInfo rule = new MappingInfo(to, from, formatter);
			Maps.Add(rule);
		}
	}

	public class MappingInfo
	{
		public MappingInfo(PropertyInfo mapTo, PropertyInfo mapFrom, string formatter)
		{
			MapTo = mapTo;
			MapFrom = mapFrom;
			Formatter = formatter;
		}
		public int Index { get; set; }
		public string Formatter { get; set; }
		public PropertyInfo MapTo { get; set; }
		public PropertyInfo MapFrom { get; set; }
	}
}

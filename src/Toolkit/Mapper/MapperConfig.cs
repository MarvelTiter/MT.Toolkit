using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MT.Toolkit.Mapper
{
	public class MapperConfig
	{
		private string prefix;
		private bool matchPrefix = false;
		public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;
		public ClassMemberHandleMode ClassMemberHandleMode { get; set; } = ClassMemberHandleMode.Ref;
		public Func<PropertyInfo, PropertyInfo, bool> PropertyMappingRule { get; set; }

		public void EnablePrefixMatch(string prefix)
		{
			this.prefix = prefix;
			matchPrefix = true;
		}

		public MapperConfig CreateMap<TFrom, TTarget>(Action<MapperRule<TFrom, TTarget>> context = null)
		{
			var map = (MapperRule<TFrom, TTarget>)MapRuleProvider.GetMapRule<TFrom, TTarget>();
			context?.Invoke(map);
			return this;
		}

		internal bool Match(PropertyInfo source, PropertyInfo target)
		{
			if (source == null || target == null)
			{
				return true;
			}
			bool matched = false;
			string sourceName = source.Name;
			string targetName = target.Name;
			matched = string.Equals(sourceName, targetName, StringComparison);
			if (matchPrefix)
			{
				var temp = string.Equals(prefix + sourceName, targetName, StringComparison) ||
					string.Equals(sourceName, prefix + targetName, StringComparison);
				matched = matched || temp;
			}
			if (PropertyMappingRule != null)
			{
				return matched && PropertyMappingRule.Invoke(source, target);
			}
			return matched;
		}
	}

	public enum ClassMemberHandleMode
	{
		None,
		NewObj,
		Ref
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using MT.KitTools.Mapper.ExpressionCore;
using MT.Toolkit.TypeExtensions;

namespace MT.Toolkit.Mapper
{
	public enum MapperType
	{
		ClassObjectToClassObject,
		ClassObjectToDictionary,
		DictionaryToClassObject
	}
	public class MappingProfile<TFrom, TTarget> : Profiles
	{
		internal MappingProfile()
		{
			types = new[] { typeof(TFrom), typeof(TTarget) };
		}

		public bool Exit(Type source, Type target)
		{
			var b1 = isTypeEquals();
			var b2 = isElementTypeEquals();
			return b1 || b2;

			bool isTypeEquals()
			{
				return ReferenceEquals(source, SourceType) && ReferenceEquals(target, TargetType);
			}

			bool isElementTypeEquals()
			{
				var requestSourceType = source.IsICollectionType() ? source.GetCollectionElementType() : source;
				var requestTargetType = target.IsICollectionType() ? target.GetCollectionElementType() : target;
				return ReferenceEquals(requestSourceType, SourceElementType) && ReferenceEquals(requestTargetType, TargetElementType);
			}
		}


		public override Delegate CreateDelegate(ActionType actionType)
		{
			MapInfo p = new MapInfo();
			p.SourceType = SourceType;
			p.TargetType = TargetType;
			p.SourceElementType = SourceElementType;
			p.TargetElementType = TargetElementType;
			p.ActionType = actionType;
			p.MapRule = MapRuleProvider.GetMapRule(p.SourceElementType, p.TargetElementType);
			var lambda = CreateExpression.ExpressionBuilder(p);
			return lambda.Compile();
		}
	}
}
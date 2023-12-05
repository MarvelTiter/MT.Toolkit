using MT.KitTools.Mapper.ExpressionCore;
using MT.Toolkit.TypeExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace MT.Toolkit.Mapper
{
	public abstract class Profiles
	{

		protected Type[] types;
		protected Type SourceType => types[0];
		protected Type TargetType => types[1];
		protected Type SourceElementType => SourceType.IsICollectionType() || SourceType.IsIEnumerableType() ? SourceType.GetCollectionElementType() : SourceType;
		protected Type TargetElementType => TargetType.IsICollectionType() || SourceType.IsIEnumerableType() ? TargetType.GetCollectionElementType() : TargetType;
		public abstract Delegate CreateDelegate(ActionType actionType);
	}

	public enum Direction
	{
		Forward,
		Backward
	}
}

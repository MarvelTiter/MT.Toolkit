using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

#nullable disable
namespace MT.Toolkit.Mapper
{
	public enum ActionType
	{
		NewObj,
		Ref
	}
	internal class MapInfo
	{
		public Type SourceType { get; set; }
		public Type TargetType { get; set; }
		public Type SourceElementType { get; set; }
		public Type TargetElementType { get; set; }
		public Expression SourceExpression { get; set; }
		public Expression TargetExpression { get; set; }
		public IMapperRule MapRule { get; set; }
		public ActionType ActionType { get; set; }
		public List<ParameterExpression> Parameters { get; set; } = new List<ParameterExpression>();
		public List<ParameterExpression> Variables { get; set; } = new List<ParameterExpression>();
	}
}
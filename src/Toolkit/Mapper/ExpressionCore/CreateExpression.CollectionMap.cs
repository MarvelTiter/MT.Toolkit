using System.Linq.Expressions;
using MT.Toolkit.Mapper;

namespace MT.KitTools.Mapper.ExpressionCore
{
    internal partial class CreateExpression
    {
        internal static Expression CollectionMap(MapInfo p)
        {
            //List<Expression> body = new List<Expression>();
            //var source = p.SourceExpression as ParameterExpression;

            //var moveNext = typeof(IEnumerator).GetMethod("MoveNext");
            //var getEnumerator = p.SourceType.GetMethod("GetEnumerator");
            //if (getEnumerator == null)
            //{
            //    getEnumerator = typeof(IEnumerable<>).MakeGenericType(p.SourceElementType).GetMethod("GetEnumerator");
            //}
            /*
            * var enumerator = source.GetEnumerator();
            * var ret = new T()
            * while(true) {
            *   if (!enumerator.MoveNext()) {
            *     goto endLabel;   
            *   }
            *   var t = enumerator.Current;
            *   MapperLink<,>.Map(t);
            * }
            * endLabel:
            */
            //LabelTarget endLabel = Expression.Label("end");

            //var linkMap = typeof(Mapper.MapperLink<,>).MakeGenericType(p.SourceElementType, p.TargetElementType).GetMethod("Map");
            //var classMap = typeof(CreateExpression).GetMethod("ClassMap"
            //    , BindingFlags.NonPublic | BindingFlags.Static
            //    , null
            //    , new[] { typeof(List<MappingInfo>), typeof(Type), typeof(Type), typeof(object) }
            //    , null);
            //var listType = typeof(List<>).MakeGenericType(p.TargetElementType);
            //var addMethod = listType.GetMethod("Add");
            //ParameterExpression listExpression = Expression.Variable(listType, "list");
            //ParameterExpression temp = Expression.Variable(p.TargetElementType, "temp");
            //body.Add(Expression.Assign(listExpression, Expression.New(listType)));

            //if (p.TargetType.IsICollectionType())
            //{
            //    List<Expression> loopBody = new List<Expression>();
            //    MethodCallExpression enumerator = Expression.Call(source, getEnumerator);
            //    ConditionalExpression loopCondition = Expression.IfThen(
            //        Expression.IsFalse(Expression.Call(enumerator, moveNext)),
            //        Expression.Break(endLabel)
            //        );
            //    MemberExpression current = Expression.Property(enumerator, "Current");
            //    var targetValue = Expression.Call(classMap
            //        , Expression.Constant(p.Rules, typeof(List<MappingInfo>))
            //        , Expression.Constant(p.SourceElementType, typeof(Type))
            //        , Expression.Constant(p.TargetElementType, typeof(Type))
            //        , current);
            //    Expression.Assign(temp, targetValue);
            //    var listAdd = Expression.Call(listExpression, addMethod, temp);
            //    loopBody.Add(loopCondition);
            //    loopBody.Add(listAdd);
            //    var loop = Expression.Loop(Expression.Block(loopBody), endLabel);
            //    body.Add(loop);
            //    //body.Add(Expression.Label(endLabel));
            //}
            //body.Add(Expression.Convert(listExpression, p.TargetType));
            //BlockExpression block = Expression.Block(new[] { listExpression }, body);
            //return block;
            return null;
        }
    }
}

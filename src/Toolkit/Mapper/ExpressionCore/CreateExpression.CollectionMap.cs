using System.Collections.Generic;
using System.Linq.Expressions;
using MT.Toolkit.Mapper;
using System;
using System.Collections;
using System.Diagnostics;

namespace MT.KitTools.Mapper.ExpressionCore
{
    internal partial class CreateExpression
    {
        internal static void CollectionMap(MapInfo p, List<Expression> body)
        {

            var moveNext = typeof(IEnumerator).GetMethod("MoveNext");
            var getEnumerator = p.SourceType.GetMethod("GetEnumerator");


            Type? enumeratorType;
            if (p.SourceType.IsGenericType)
            {
                var innerType = p.SourceType.GetNestedType("Enumerator");
                if (innerType != null)
                {
                    enumeratorType = innerType.MakeGenericType(p.SourceElementType);
                }
                else
                {
                    enumeratorType = typeof(IEnumerator<>).MakeGenericType(p.SourceElementType);
                }
            }
            else if (p.SourceType.IsArray)
            {
                enumeratorType = typeof(IEnumerator);
            }
            else
            {
                throw new Exception("Unkonw Enumerator");
            }

            /*
            * var enumerator = source.GetEnumerator();
            * var ret = new T()
            * while(true) {
            *   if (enumerator.MoveNext()) {
            *       var t = enumerator.Current;
            *       var n = MapperLink<,>.Map(t);
            *       ret.Add(n);
            *       // ret.Add(MapperLink<,>.Map(enumerator.Current));
            *   }
            *   else
            *   {
            *       goto endLabel
            *   }
            * }
            * endLabel:
            */
            LabelTarget endLabel = Expression.Label("end");
            var linkMap = typeof(MapperExtensions.MapperLink<,>).MakeGenericType(p.SourceElementType, p.TargetElementType).GetMethod("Create");
            var listType = typeof(List<>).MakeGenericType(p.TargetElementType);
            var templist = Expression.Variable(listType);
            body.Add(Expression.Assign(templist, Expression.New(listType)));
            p.Variables.Add(templist);
            var addMethod = listType.GetMethod("Add");

            var enumeratorExpression = Expression.Variable(enumeratorType, "enumerator");
            p.Variables.Add(enumeratorExpression);
            body.Add(Expression.Assign(enumeratorExpression, Expression.Call(p.SourceExpression, getEnumerator!)));
            var loop = Expression.Loop(Expression.Block(Expression.IfThenElse(Expression.Call(enumeratorExpression, moveNext!)
                 , Expression.Block(Expression.Call(templist, addMethod!, Expression.Call(linkMap!, ValueExpression(enumeratorExpression, p.SourceElementType))
                 ))
                 , Expression.Break(endLabel)
                 )), endLabel);
            ;
            body.Add(loop);

            if (p.TargetExpression == null)
            {
                p.TargetExpression = Expression.Variable(p.TargetType, "tar");
                p.Variables.Add(p.TargetExpression as ParameterExpression);
            }

            if (p.ActionType == ActionType.NewObj)
            {
                if (p.TargetType.IsArray)
                {
                    var toArray = listType.GetMethod("ToArray")!;
                    body.Add(Expression.Assign(p.TargetExpression, Expression.Call(templist, toArray)));
                }
                else
                {
                    body.Add(Expression.Assign(p.TargetExpression, templist));
                    body.Add(Expression.Convert(p.TargetExpression, p.TargetType));
                }
            }


            Expression ValueExpression(Expression enumerator, Type targetType)
            {
                var current = Expression.Property(enumerator, nameof(IEnumerator.Current));
                return Expression.Convert(current, targetType);
            }

        }

        //private static void CreateArray(MapInfo p, List<Expression> body)
        //{
        //    if (p.TargetExpression == null)
        //    {
        //        p.TargetExpression = Expression.Variable(p.TargetType, "tar");
        //        body.Add(Expression.Assign(p.TargetExpression, Expression.NewArrayInit(p.SourceElementType)));
        //        p.Variables.Add(p.TargetExpression as ParameterExpression);
        //    }
        //    /*
        //     * var length = source.Length;
        //     * var ret = new T[length];
        //     * while(true)
        //     * {
        //     *     if (
        //     * }
        //     */
        //    var linkMap = typeof(MapperExtensions.MapperLink<,>).MakeGenericType(p.SourceElementType, p.TargetElementType).GetMethod("Create");

        //    var index = Expression.Variable(typeof(int));
        //    p.Variables.Add(index);
        //    var length = Expression.Property(p.SourceExpression, nameof(Array.Length));
        //    var breakLabel = Expression.Label();
        //    Expression.Loop(Expression.Block(
        //        Expression.IfThenElse(
        //            Expression.LessThan(index, length),
        //      Expression.Block(Expression.arr
        //          ,Expression.PostIncrementAssign(index)),
        //      Expression.Break(breakLabel)
        //            )
        //        ), breakLabel);
        //}

        private static void CreateList(MapInfo p, List<Expression> body)
        {

        }
    }
}

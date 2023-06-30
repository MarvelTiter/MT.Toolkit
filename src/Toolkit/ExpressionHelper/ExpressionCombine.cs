using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MT.Toolkit.ExpressionHelper
{
    public static class ExpressionCombine
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> other)
        {
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.AndAlso, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> AndAlsoIf<T>(this Expression<Func<T, bool>> self, bool condition, Expression<Func<T, bool>> other)
        {
            if (!condition) return self;
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.AndAlso, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> AndAlsoIf<T>(this Expression<Func<T, bool>> self, Func<bool> condition, Expression<Func<T, bool>> other)
        {
            if (!condition.Invoke()) return self;
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.AndAlso, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> self, Expression<Func<T, bool>> other)
        {
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.OrElse, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }

        public static Expression<Func<T, bool>> OrElseIf<T>(this Expression<Func<T, bool>> self, bool condition, Expression<Func<T, bool>> other)
        {
            if (!condition) return self;
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.OrElse, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
        public static Expression<Func<T, bool>> OrElseIf<T>(this Expression<Func<T, bool>> self, Func<bool> condition, Expression<Func<T, bool>> other)
        {
            if (!condition.Invoke()) return self;
            var p = self.Parameters;
            var body = Expression.MakeBinary(ExpressionType.OrElse, self.Body, other.Body);
            return Expression.Lambda<Func<T, bool>>(body, p);
        }
    }
}

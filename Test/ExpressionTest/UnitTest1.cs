using System.Linq.Expressions;
using MT.Toolkit.ExpressionHelper;
namespace ExpressionTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            Expression<Func<string, bool>> exp1 = s => s.Length > 10;
            Expression<Func<string, bool>> exp2 = s => s.Length < 20;
            var nExp = exp1.AndAlso(exp2);
            Console.WriteLine(nExp);
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            Expression<Func<string, bool>> exp1 = s => s.Length > 10;
            Expression<Func<string, bool>> exp2 = s => s.Length < 20;
            var nExp = exp1.OrElse(exp2);
            Console.WriteLine(nExp);
            Assert.Pass();
        }
    }
}
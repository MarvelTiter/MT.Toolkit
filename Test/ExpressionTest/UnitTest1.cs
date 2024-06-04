using System.Linq.Expressions;
using MT.Toolkit.ExpressionHelper;
using MT.Toolkit.ReflectionExtension;
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

        class User
        {
            public string Name { get; set; } = "Hello";
        }

        [Test]
        public void CreateGetterSetterTest()
        {
            var u = new User();
            var getter = u.GetPropertyAccessor<string>("Name");
            Assert.IsTrue(getter.Invoke(u) == "Hello");

            var setter = u.GetPropertySetter("Name");
            setter.Invoke(u, "World");
            Assert.IsTrue(getter.Invoke(u) == "World");
        }

    }
}
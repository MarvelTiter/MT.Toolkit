using MT.Toolkit.Mapper;
using System.Dynamic;

namespace MapperTest
{
    [TestClass]
    public class UnitTest1
    {

        class CModel
        {
            public int P1 { get; set; }
            public string P2 { get; set; }
            public bool P3 { get; set; }
            public DateTime P4 { get; set; }
        }

        [TestMethod]
        public void MapperFromIDictionary()
        {
            var source = new ExpandoObject();
            source.TryAdd("P1", 1);
            source.TryAdd("P2", "hello");
            source.TryAdd("P3", true);
            source.TryAdd("P4", DateTime.Now);

            var ret = Mapper.Map<IDictionary<string, object?>, CModel>(source);
        }
    }
}
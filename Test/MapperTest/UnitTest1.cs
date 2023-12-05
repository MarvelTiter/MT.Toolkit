using MT.Toolkit.Mapper;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace MapperTest
{
    [TestClass]
    public class UnitTest1
    {

        class CModel
        {
            public CModel()
            {
                P1 = Random.Shared.Next(0, 100);
                P4 = DateTime.Now;
                P2 = P1?.ToString() ?? "Error";
                P3 = P1 > 50;
            }
            public int? P1 { get; set; }
            public string P2 { get; set; }
            public bool P3 { get; set; }
            public DateTime P4 { get; set; }
        }

        [TestMethod]
        public void MapperFromIDictionary()
        {
            var d = DateTime.Now;
            var source = new ExpandoObject();
            source.TryAdd("P1", "");
            source.TryAdd("P2", "hello");
            source.TryAdd("P3", "true");
            source.TryAdd("P4", d.ToString("yyyy-MM-dd HH:mm:ss"));

            var ret = Mapper.Map<IDictionary<string, object?>, CModel>(source);

            Assert.IsTrue(!ret.P1.HasValue);
            Assert.IsTrue(ret.P2 == "hello");
            Assert.IsTrue(ret.P3 == true);
            Assert.IsTrue(ret.P4.ToString() == d.ToString());
        }

        [TestMethod]
        public void MapperCollectionTest_ListToList()
        {
            var list = new List<CModel>() { new(), new() };
            var nlist = Mapper.Map(list);
            for (int i = 0; i < nlist.Count; i++)
            {
                Assert.IsTrue(list[i].P1 == nlist[i].P1);
                Assert.IsTrue(list[i].P2 == nlist[i].P2);
                Assert.IsTrue(list[i].P3 == nlist[i].P3);
                Assert.IsTrue(list[i].P4 == nlist[i].P4);
            }
        }

        [TestMethod]
        public void MapperCollectionTest_ArrayToArray()
        {
            var list = new CModel[] { new(), new() };
            var nlist = Mapper.Map(list);
            for (int i = 0; i < list.Length; i++)
            {
                Assert.IsTrue(list[i].P1 == nlist[i].P1);
                Assert.IsTrue(list[i].P2 == nlist[i].P2);
                Assert.IsTrue(list[i].P3 == nlist[i].P3);
                Assert.IsTrue(list[i].P4 == nlist[i].P4);
            }
        }

        [TestMethod]
        public void MapperCollection_ListToIEnumerator()
        {
            var list = new List<CModel>() { new(), new() };
            var nlist = Mapper.Map<List<CModel>, IEnumerable<CModel>>(list);
            var i = 0;
            foreach (var item in nlist)
            {
                Assert.IsTrue(list[i].P1 == item.P1);
                Assert.IsTrue(list[i].P2 == item.P2);
                Assert.IsTrue(list[i].P3 == item.P3);
                Assert.IsTrue(list[i].P4 == item.P4);
                i++;
            }
        }

        [TestMethod]
        public void MapperCollection_IEnumeratorToList()
        {
            var list = new List<CModel>() { new(), new() };
            var nlist = Mapper.Map<IEnumerable<CModel>, List<CModel>>(list.AsEnumerable());
            var i = 0;
            foreach (var item in nlist)
            {
                Assert.IsTrue(list[i].P1 == item.P1);
                Assert.IsTrue(list[i].P2 == item.P2);
                Assert.IsTrue(list[i].P3 == item.P3);
                Assert.IsTrue(list[i].P4 == item.P4);
                i++;
            }
        }

        [TestMethod]
        public void MapperCollection_ArrayToList()
        {
            var list = new CModel[] { new(), new() };
            var nlist = Mapper.Map<CModel[], List<CModel>>(list);
            for (int i = 0; i < list.Length; i++)
            {
                Assert.IsTrue(list[i].P1 == nlist[i].P1);
                Assert.IsTrue(list[i].P2 == nlist[i].P2);
                Assert.IsTrue(list[i].P3 == nlist[i].P3);
                Assert.IsTrue(list[i].P4 == nlist[i].P4);
            }
        }

    }
}
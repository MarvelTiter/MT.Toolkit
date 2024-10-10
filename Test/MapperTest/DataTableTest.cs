using MT.Toolkit.DataTableExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapperTest;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
    public DateTime Birth { get; set; }
}

[TestClass]
public class DataTableTest
{
    [TestMethod]
    public void ToDataTable()
    {
        List<User> users = [
            new User { Age = 1, Name = "Haha", Birth = DateTime.Now },
            new User { Age = 2, Name = "Haha", Birth = DateTime.Now },
            new User { Age = 3, Name = "Haha", Birth = DateTime.Now },
            new User { Age = 4, Name = "Haha", Birth = DateTime.Now }
            ];

        var dt = users.ToDataTable();
    }
}

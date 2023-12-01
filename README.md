# 目的

### ExpressionTree 的学习

# 开发目标

# SoapHelper

```CSharp
// 容器中使用
IServiceCollection services = new ServiceCollection();
// 内部需要IHttpClientFactory创建HttpClient
services.AddHttpClient();
// 配置SoapWebService节点
services.AddSoapServiceHelper(manager =>
{
    manager.AddSoapService("Main", config =>
    {
        config.Url = "";
        config.RequestNamespace = "";
        config.ResponseNamespace = "";
    }).AddSoapService("Sec", config =>
    {
        config.Url = "";
        config.RequestNamespace = "";
        config.ResponseNamespace = "";
    })
    .SetDefault("Sec");
});

var provider = services.BuildServiceProvider();
// 获取工厂
var fac = provider.GetService<ISoapServiceFactory>();
// 通过注册时的key创建对应的SoapService
var main = fac!.GetSoapService("Main");

var ris = fac!.GetSoapService("Sec");

var login = await ris.SendAsync("Login", new()
{
    ["szUser"] = "admin",
    ["szPass"] = "hgbanner"
});

var response = await main.SendAsync("UpdateLst", new()
{
    ["lTick"] = 0,
    ["typeid"] = 1,
    ["MyGuid"] = login.ReadReturnValueAsXml()?.GetValue("//Guid")!
});
var d = response.ReadReturnValueAsXml()?.AsDynamic();
```

```CSharp
// 直接new一个SoapService来使用
 var client = new HttpClient();
 var ws = new SoapService(client, "", "");
 var response = await ws.SendAsync("Login", new()
 {
     ["szUser"] = "admin",
     ["szPass"] = "admin123"
 });
```

# SimpleLogger

```CSharp
// 1. 依赖注入
builder.Logging.AddSimpleLogger(config =>
{
	config.EnableAllDefault();
});

// 2. 独立使用
// 2.1 配置
SimpleLogger.Config(config =>
{
	config.EnableAllDefault();
});
// 2.2 使用
SimpleLogger.LogInformation("Hello");
SimpleLogger.LogDebug("Debug");
```

# 基本拓展

## DataTable 拓展方法

```CSharp
// 是否有数据
bool HasRows(this DataTable dt);
// 转实体
IEnumerable<T> ToEnumerable<T>(this DataTable self, bool mapAllFields = false);
// 按条件转实体
IEnumerable<T> Select<T>(this DataTable self, Func<DataRow, bool> filter, bool mapAllFields = false);
IEnumerable<T> Select<T>(this DataTable self, Func<T, bool> filter, bool mapAllFields = false);
// 行转实体
T Parse<T>(this DataRow row, bool mapAllFields);
// 从行中获取值
T Value<T>(this DataRow self, string key);
// 从DataTable中给T赋值（首行）
void MapFromTable<T>(this T self, DataTable source);
```

## DateTime 拓展方法

```CSharp
// 返回一天的零点(yyyyMMdd 000000)
DateTime DayStart(this DateTime self);
string DayStartStr(this DateTime self, string format = "yyyy-MM-dd HH:mm:ss");
// 返回一天的末点(yyyyMMdd 23:59:59)
DateTime DayEnd(this DateTime self);
string DayEndStr(this DateTime self, string format = "yyyy-MM-dd HH:mm:ss");
```

## String 拓展

```CSharp
// 根据条件决定是否返回字符串
string If(this string self, Func<bool> condition);
string If(this string self, bool condition);
// 字符串是否合法
bool IsEnable(this string self, Func<string, bool> rule = null);
// 字符串是否是数字
bool IsNumeric<T>(this string self, out T value) where T : struct;
bool IsNumeric(this string self);
// 追加到StringBuilder中
void AppendTo(this string str, StringBuilder builder);
```

## Enum 拓展

```CSharp
// 返回System.ComponentModel.DataAnnotations.DisplayAttribute的Name
string GetDisplayName<T>(this T @enum) where T : Enum;
```

## Type 拓展

```CSharp
bool IsDictionary(this Type type);
bool IsIEnumerableType(this Type type);
bool IsICollectionType(this Type type);
bool IsNullableType(this Type type);
Type GetCollectionElementType(this Type type);
bool IsGenericEnumerableType(this Type type);
```

## 机器相关

```CSharp
// 1. Cpu相关
// 1.1 获取CPU核心数
CpuHelper.ProcessorCount();
// 1.2 获取CPU使用情况
CpuHelper.CpuTotalUsage();
// 1.3 获取当前进程CPU使用情况
CpuHelper.CpuProcessUsageAsync()

// 2. 内存相关
// 3. 硬盘相关
// 4. 系统信息
```

# Mapper

## 配置示例

```CSharp
Mapper.Default.Configuration(config =>
{
    config.StringComparison = System.StringComparison.OrdinalIgnoreCase | System.StringComparison.CurrentCulture;
    config.EnablePrefixMatch("TEST_");
});
Mapper.Default.CreateMap<User, UserDTO>(profile =>
{
    profile.Mapping((u, ut) =>
    {
        ut.NA = $"{u.Name} => {u.Age}";
    });
}).CreateMap<UserDTO, User>(profile =>
{
    profile.Mapping((ut, u) =>
    {
        var m = Regex.Match(ut.NA, "(\\w+) => (\\d+)");
        if (m.Success)
        {
            u.Name = m.Groups[1].Value;
            m.Groups[2].Value.IsNumeric<int>(out var a);
            u.Age = a;
        }
    });
});
```

## 使用

```CSharp
Mapper.Map<User, UserDTO>(user);
```

# ReflectionHelper

## 调用对象实例方法

```CSharp
void Invoke(this object obj, string methodName, params object[] args);
T Invoke<T>(this object obj, string methodName, params object[] args);
```

## 调用类型静态方法

```CSharp
void Invoke(this Type type, string methodName, params object[] args);
T Invoke<T>(this Type type, string methodName, params object[] args);
```

## 设置对象属性值

```CSharp
void Set<T>(this object self, PropertyInfo prop, T value);
void Set<T>(this object self, string propName, T value);
```

## 类型转换

```CSharp
T Parse<T>(this object self);
```

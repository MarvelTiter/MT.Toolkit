# 在DotNet中使用HttpClient调用WebService服务

## 概述

构建Soap请求报文调用WebService接口服务，支持多端点配置，支持请求队列和并发配置

## 使用方式

### 配置服务
```csharp
builder.Services.AddSoapServiceHelper(m =>
{
    m.AddSoapService("Service1"), config =>
    {
        config.Url = "Service1 WebServiceUrl";
        config.ConcurrencyLimit = maxConcurrent;
        config.QueueCapacity = maxQueueLimit;
        config.RequestNamespace = "RequestNamespace";
        config.ResponseNamespace = "ResponseNamespace";
    }).AddSoapService("Service2"), config =>
    {
        config.Url = "Service2 WebServiceUrl";
        config.ConcurrencyLimit = maxConcurrent;
        config.QueueCapacity = maxQueueLimit;
        config.RequestNamespace = "RequestNamespace";
        config.ResponseNamespace = "ResponseNamespace";
    }).SetDefault("Service1");
});
```

### 获取`ISoapService`
#### 先获取`ISoapServiceFactory`
```csharp
public class TestClass(ISoapServiceFactory soapFactory)
{

}
```
#### 通过`ISoapServiceFactory`获取`ISoapService`
```csharp
ISoapService service = soapFactory.GetSoapService("Service1");
```

#### 使用`ISoapService`发送请求
```csharp
SoapResponse response = await service.SendAsync("MethodName", new()
{
    ["Param1"] = value1,
    ["Param2"] = value2,
    ["Param3"] = value3
});
```
#### 处理结果
|属性/方法|说明|备注|
|-|-|-|
|Success|调用是否成功
|Exception|异常对象
|RawContent|响应的完整报文
|RawValue|响应的Body内容
|RequestContent|请求的原始报文
|ReadReturnValue|返回接口签名中的返回值|具有泛型和非泛型版本，泛型可以尝试转化成对应的数据类型，非泛型返回dynamic
|ReadReturnValueAsXml|返回接口签名中的返回值|返回值类型是XElement?
|ReadReturnValueAsDataTable|如果返回值是DataTable，尝试转为DataTable
|ReadParameterReturnValue|out参数或者ref参数的返回值|具有泛型和非泛型版本，泛型可以尝试转化成对应的数据类型，非泛型返回dynamic
|ReadParameterReturnValueAsXml|out参数或者ref参数的返回值|返回值类型是XElement?
|ReadParameterReturnValueAsDataTable|out参数或者ref参数的返回值|如果返回值是DataTable，尝试转为DataTable
|GetValue|根据XPath返回节点内容|需要加上`r`命名空间
|GetNode|根据XPath返回节点|需要加上`r`命名空间
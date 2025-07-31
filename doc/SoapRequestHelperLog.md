## 版本功能更新日志

### v0.0.2
- ⚡️抽象`HttpRequestChannel`, 分离了队列容量(QueueCapacity)和并发限制(ConcurrencyLimit), 队列只关注请求的发送, SOAP协议相关处理提取到`SoapService`类中

### v0.0.1
- ⚡️将Toolkit中的HttpHelper提取`SoapRequestHelper`nuget包发布
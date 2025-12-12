### v0.0.2
- 🛠重构日志配置功能，适配原生Logging的配置方式
- 🛠在原生Logging配置的基础上，增加自定义配置项。查看`AddLocalFileLogger`和`AddDbLogger`的可选参数
- ⚡️优化文件的日志构建逻辑，减少不必要的字符串拼接

  | 日志输出优先级(ProviderScope是`LocalFile`或者`DatabaseLog`) | 示例    | 
  |---------------------------------|-------|
  | 最高：Logging:ProviderScope下的Category设置    | TestWeb的优先级最高 |
  | 次高：Logging:ProviderScope下的MinLevel设置    | LocalFile:MinLevel高于LogLevel:Microsoft |
  | 次高：全局的Category设置高于MinLevel     | LogLevel:Microsoft.AspNetCore高于MinLevel |
  | LogLevel:Default | 永远不记录 |
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Error"
    },
    "LocalFile": {
      "LogLevel": {
        "TestWeb": "Information"
      },
      "FileSavedDays": 5,
      "MinLevel: "Warning"
    }
  }
}
```
### v0.0.1
- ⚡️将Toolkit中的HttpHelper提取`LoggerProviderExtensions`nuget包发布
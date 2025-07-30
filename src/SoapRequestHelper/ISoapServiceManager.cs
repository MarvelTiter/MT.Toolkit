using System;
using System.Collections.Concurrent;
namespace SoapRequestHelper;

/// <summary>
/// SOAP服务配置器
/// </summary>
public interface ISoapServiceManager
{
    internal ConcurrentDictionary<string, SoapServiceConfiguration> Configs { get; }
    internal string? DefaultKey {  get; }
    /// <summary>
    /// 设置默认的KEY
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ISoapServiceManager SetDefault(string key);
    /// <summary>
    /// 添加SOAP服务
    /// </summary>
    /// <param name="name">配置名称</param>
    /// <param name="config"></param>
    /// <returns></returns>
    ISoapServiceManager AddSoapService(string name, Action<SoapServiceConfiguration> config);
}
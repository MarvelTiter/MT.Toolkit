using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
namespace SoapRequestHelper;

/// <summary>
/// 配置的Soap服务
/// </summary>
public interface ISoapService : IAsyncDisposable
{
    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="methodName">方法名</param>
    /// <param name="args">参数字典</param>
    /// <param name="cancellationToken">取消token</param>
    /// <returns></returns>
    ValueTask<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="client">自定义的HttpClient</param>
    /// <param name="methodName">方法名</param>
    /// <param name="args">参数字典</param>
    /// <param name="cancellationToken">取消token</param>
    /// <returns></returns>
    ValueTask<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default);
}

using System;

namespace MT.Toolkit.HttpHelper
{
    /// <summary>
    /// SOAP服务工厂
    /// </summary>
    public interface ISoapServiceFactory : IDisposable
    {
        /// <summary>
        /// 获取默认服务, 通过<see cref="ISoapServiceManager.SetDefault(string)"/>设置
        /// </summary>
        ISoapService? Default { get; }
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <param name="key">服务配置名称</param>
        /// <returns></returns>
        ISoapService GetSoapService(string key);
    }
}

using System;

namespace MT.Toolkit.HttpHelper
{
    public interface ISoapServiceFactory : IDisposable
    {
        ISoapService? Default { get; }
        ISoapService GetSoapService(string key);
    }
}

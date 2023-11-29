#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
namespace MT.Toolkit.HttpHelper
{
    public interface ISoapServiceFactory
    {
        ISoapService? Default { get; }
        ISoapService GetSoapService(string key);
    }
}
#endif
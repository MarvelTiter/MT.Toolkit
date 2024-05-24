namespace MT.Toolkit.HttpHelper
{
    public interface ISoapServiceFactory
    {
        ISoapService? Default { get; }
        ISoapService GetSoapService(string key);
    }
}

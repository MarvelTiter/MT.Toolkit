using System.Xml.Linq;


#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
namespace MT.Toolkit.HttpHelper
{
    public class SoapResponse
    {
        private readonly string? rawValue;

        public SoapResponse(string? rawValue)
        {
            this.rawValue = rawValue;
        }

        public string? RawValue => rawValue;

        private XDocument? xml;
        public XDocument? Xml
        {
            get
            {
                try
                {
                    if (rawValue != null && xml == null)
                        xml = XDocument.Parse(rawValue);
                }
                catch
                {
                }
                return xml;
            }
        }
    }
}
#endif
#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace MT.Toolkit.HttpHelper
{

    public record XmlnsItem
    {
        public XmlnsItem(string Prefix, string Uri)
        {
            this.Prefix = Prefix;
            this.Uri = Uri;
        }

        public string Prefix { get; }
        public string Uri { get; }
    }
    public class SoapResponse
    {
        private readonly string? rawValue;
        private XmlNamespaceManager? nsManager;
        //private XmlReader? xmlReader;
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
                    {
                        using var reader = new StringReader(rawValue);
                        using var xmlReader = XmlReader.Create(reader);
                        nsManager = new XmlNamespaceManager(xmlReader.NameTable);
                        xml = XDocument.Load(xmlReader);
                    }
                }
                catch
                {
                }
                return xml;
            }
        }

        public string? GetValue(string expression, Func<IEnumerable<XmlnsItem>>? configNamespaces = null)
        {
            var ns = configNamespaces?.Invoke() ?? Enumerable.Empty<XmlnsItem>();
            foreach (var nsItem in ns)
            {
                nsManager?.AddNamespace(nsItem.Prefix, nsItem.Uri);
            }
            var value = Xml?.XPathSelectElement(expression, nsManager)?.Value;
            return value;
        }

        public T? GetValue<T>(string expression, Func<IEnumerable<XmlnsItem>>? configNamespaces = null)
        {
            var str = GetValue(expression, configNamespaces);
            if (string.IsNullOrEmpty(str)) return default;
            var ret = Convert.ChangeType(str, typeof(T));
            if (ret == null) return default;
            return (T?)ret;
        }
    }
}
#endif
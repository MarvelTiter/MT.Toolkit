using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Linq;
using System.Data.SqlTypes;



#if NET6_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER
using System.Net.Http;
using System.Xml;
using System.Xml.XPath;
namespace MT.Toolkit.HttpHelper
{
    public class SoapService : ISoapService
    {
        private readonly HttpClient client;
        private readonly string url;
        private readonly SoapVersion version;
        private readonly string requestNamespace;
        private readonly string responseNamespace;
        private bool disposedValue;

        private string EnvelopeNs
        {
            get
            {
                if (version != SoapVersion.Soap12)
                {
                    return "http://schemas.xmlsoap.org/soap/envelope/";
                }

                return "http://www.w3.org/2003/05/soap-envelope";
            }
        }

        private string EncodingNs
        {
            get
            {
                if (version != SoapVersion.Soap12)
                {
                    return "http://schemas.xmlsoap.org/soap/encoding/";
                }

                return "http://www.w3.org/2003/05/soap-encoding";
            }
        }

        private string HttpContentType
        {
            get
            {
                if (version != SoapVersion.Soap12)
                {
                    return "text/xml";
                }

                return "application/soap+xml";
            }
        }

        public SoapService(HttpClient client, SoapServiceConfiguration configuration)
        {
            this.client = client;
            this.url = configuration.Url ?? throw new ArgumentNullException();
            this.version = configuration.Version ?? SoapVersion.Soap11;
            this.requestNamespace = configuration.RequestNamespace ?? "http://tempuri.org/";
            this.responseNamespace = configuration.ResponseNamespace ?? "http://tempuri.org/";
        }

        public SoapService(HttpClient client, string url) : this(client, url, SoapVersion.Soap11, "http://tempuri.org/")
        {

        }

        public SoapService(HttpClient client, string url, string @namespace) : this(client, url, SoapVersion.Soap11, @namespace)
        {

        }

        public SoapService(HttpClient client, string url, SoapVersion version, string @namespace) : this(client, url, version, @namespace, @namespace)
        {

        }

        public SoapService(HttpClient client, string url, SoapVersion version, string requestNamespace, string responseNamespace)
        {
            this.client = client;
            this.url = url;
            this.version = version;
            this.requestNamespace = requestNamespace.EndsWith('/') ? requestNamespace : requestNamespace + '/';
            this.responseNamespace = responseNamespace.EndsWith('/') ? requestNamespace : requestNamespace + '/';
        }
        public async Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null)
        {
            StringBuilder contentString = new StringBuilder();
            if (args != null)
            {
                foreach (var item in args)
                {
                    contentString.Append($"<{item.Key}><![CDATA[{item.Value}]]></{item.Key}>");
                }
            }
            string content = $@"<soapenv:Envelope xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                   <soapenv:Body>
                     <{methodName} xmlns=""{requestNamespace}"">
                         {contentString}
                     </{methodName}>
                 </soapenv:Body>
              </soapenv:Envelope>";
            if (version == SoapVersion.Soap11)
            {
                client.DefaultRequestHeaders.Remove("SOAPAction");
                client.DefaultRequestHeaders.Add("SOAPAction", $"{requestNamespace}{methodName}");
            }
            HttpContent  httpContent = new StringContent(content, Encoding.UTF8, HttpContentType);
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, httpContent);

                // 得到返回的结果，注意该结果是基于XML格式的，最后按照约定解析该XML格式中的内容即可。
                var result = await response.Content.ReadAsStreamAsync();
                var rawContent = await response.Content.ReadAsStringAsync();
                // 解析内容
                //using var reader = new StringReader(result);
                using var xmlReader = XmlReader.Create(result);
                var doc = XDocument.Load(xmlReader);
                XmlNameTable nameTable = xmlReader.NameTable;
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
                namespaceManager.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                if (!string.IsNullOrEmpty(responseNamespace))
                {
                    namespaceManager.AddNamespace("r", responseNamespace);
                }
                var innerXml = doc.XPathSelectElement($"//soap:Body/r:{methodName}Response", namespaceManager)?.ToString();
                return new SoapResponse(rawContent, innerXml, namespaceManager, methodName);

            }
            catch (Exception ex)
            {
                return new SoapResponse(ex);
            }

        }

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (!disposedValue)
        //    {
        //        if (disposing)
        //        {
        //            client?.Dispose();
        //        }
        //        disposedValue = true;
        //    }
        //}

        //public void Dispose()
        //{
        //    Dispose(disposing: true);
        //    GC.SuppressFinalize(this);
        //}
    }
}
#endif
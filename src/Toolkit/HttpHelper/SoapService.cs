using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Xml;
using System.Xml.XPath;
namespace MT.Toolkit.HttpHelper
{
    public class SoapService : ISoapService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly SoapServiceConfiguration? configuration;
        private readonly ILogger<SoapService>? logger;
        private readonly IServiceProvider? services;
        private readonly string url;
        private readonly SoapVersion version;
        private readonly string requestNamespace;
        private readonly string responseNamespace;
        private bool disposedValue;
        private HttpClient httpClient;
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

        public SoapService(IHttpClientFactory clientFactory, SoapServiceConfiguration configuration, ILogger<SoapService> logger, IServiceProvider services)
        {
            this.clientFactory = clientFactory;
            this.configuration = configuration;
            this.logger = logger;
            this.services = services;
            this.url = configuration.Url ?? throw new ArgumentNullException();
            this.version = configuration.Version ?? SoapVersion.Soap11;
            this.requestNamespace = configuration.RequestNamespace ?? "http://tempuri.org/";
            this.responseNamespace = configuration.ResponseNamespace ?? "http://tempuri.org/";
            this.httpClient = configuration.ClientProvider?.Invoke() ?? clientFactory.CreateClient();
        }

        public SoapService(IHttpClientFactory clientFactory, string url) : this(clientFactory, url, SoapVersion.Soap11, "http://tempuri.org/")
        {

        }

        public SoapService(IHttpClientFactory clientFactory, string url, string @namespace) : this(clientFactory, url, SoapVersion.Soap11, @namespace)
        {

        }

        public SoapService(IHttpClientFactory clientFactory, string url, SoapVersion version, string @namespace) : this(clientFactory, url, version, @namespace, @namespace)
        {

        }

        public SoapService(IHttpClientFactory clientFactory, string url, SoapVersion version, string requestNamespace, string responseNamespace)
        {
            this.clientFactory = clientFactory;
            this.url = url;
            this.version = version;
            this.requestNamespace = requestNamespace.EndsWith("/") ? requestNamespace : requestNamespace + '/';
            this.responseNamespace = responseNamespace.EndsWith("/") ? requestNamespace : requestNamespace + '/';
            this.httpClient = configuration?.ClientProvider?.Invoke() ?? clientFactory.CreateClient();
        }

        //private HttpClient GetClient(string methodName)
        //{
        //    if (configuration?.ClientProvider == null)
        //    {
        //        return clientFactory.CreateClient();
        //    }
        //    return configuration.ClientProvider.Invoke(new ProviderContext(clientFactory, services, methodName));
        //}

        public Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null) => SendAsync(httpClient, methodName, args);

        public async Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null)
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
            HttpContent httpContent = new StringContent(content, Encoding.UTF8, HttpContentType);
            try
            {
                //Stopwatch sw = Stopwatch.StartNew();
                //sw.Stop();
                //logger?.LogError($"SOAP Action: {methodName}, Elapsed {sw.ElapsedMilliseconds} ms");
                HttpResponseMessage response = await client.PostAsync(url, httpContent).ConfigureAwait(false);
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

        //private async Task<Stream> UseWebRequest(string content, out string rawContent)
        //{
        //    //发起请求
        //    WebRequest webRequest = WebRequest.Create(url);
        //    webRequest.ContentType = "text/xml; charset=utf-8";
        //    webRequest.Method = "POST";
        //    using (Stream requestStream = webRequest.GetRequestStream())
        //    {
        //        byte[] paramBytes = Encoding.UTF8.GetBytes(content);
        //        requestStream.Write(paramBytes, 0, paramBytes.Length);
        //    }
        //    //响应
        //    try
        //    {
        //        WebResponse webResponse = webRequest.GetResponse();
        //        var stream = webResponse.GetResponseStream();
        //        using var stringReader = new StreamReader(stream, Encoding.UTF8);
        //        rawContent = await stringReader.ReadToEndAsync();
        //        stream.Seek(0, SeekOrigin.Begin);
        //        return stream;
        //    }
        //    catch (WebException ex)
        //    {
        //        rawContent = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
        //        return null;
        //    }
        //}

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
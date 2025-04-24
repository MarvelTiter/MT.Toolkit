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
using System.IO;
using System.Linq;
using MT.Toolkit.XmlHelper;
namespace MT.Toolkit.HttpHelper;

public class SoapService : ISoapService//, IDisposable
{
    private readonly IHttpClientFactory clientFactory;
    private readonly string? url;
    private readonly SoapVersion? version;
    private readonly string? requestNamespace;
    private readonly string? responseNamespace;
    private readonly SoapServiceConfiguration? configuration;
    //private bool disposedValue;

    private string Url => configuration?.Url ?? url ?? throw new ArgumentNullException();
    private SoapVersion Version => configuration?.Version ?? version ?? SoapVersion.Soap11;
    private string RequestNamespace => configuration?.RequestNamespace ?? requestNamespace ?? "http://tempuri.org/";
    private string ResponseNamespace => configuration?.ResponseNamespace ?? responseNamespace ?? "http://tempuri.org/";
    private string EnvelopeNs
    {
        get
        {
            if (Version != SoapVersion.Soap12)
            {
                return "http://schemas.xmlsoap.org/soap/envelope/";
            }

            return "http://www.w3.org/2003/05/soap-envelope";
        }
    }

    private string NpAlia
    {
        get
        {
            if (Version != SoapVersion.Soap12)
            {
                return "soap";
            }
            return "soap12";
        }
    }

    private string EncodingNs
    {
        get
        {
            if (Version != SoapVersion.Soap12)
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
            if (Version != SoapVersion.Soap12)
            {
                return "text/xml";
            }

            return "application/soap+xml";
        }
    }

    public SoapService(IHttpClientFactory clientFactory, SoapServiceConfiguration configuration)
    {
        this.clientFactory = clientFactory;
        this.configuration = configuration;
        //this.httpClient = this.clientFactory.CreateClient(configuration.Name);
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
        //this.httpClient = clientFactory.CreateClient();
    }

    private static string FormatValue(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }
        if (value is byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        if (value is DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }
        return $"{value}";
    }

    public Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null)
    {
        var client = clientFactory.CreateClient(configuration?.Name ?? "");
        return SendAsync(client, methodName, args);
    }

    public async Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null)
    {
        StringBuilder contentString = new StringBuilder();
        if (args != null)
        {
            foreach (var item in args)
            {
                contentString.Append($"<{item.Key}><![CDATA[{FormatValue(item.Value)}]]></{item.Key}>");
            }
        }
        string content = $@"<soapenv:Envelope xmlns:soapenv=""{EnvelopeNs}"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                   <soapenv:Body>
                     <{methodName} xmlns=""{RequestNamespace}"">
                         {contentString}
                     </{methodName}>
                 </soapenv:Body>
              </soapenv:Envelope>";
        if (Version == SoapVersion.Soap11)
        {
            client.DefaultRequestHeaders.Remove("SOAPAction");
            client.DefaultRequestHeaders.Add("SOAPAction", $"{RequestNamespace}{methodName}");
        }
        string? rawContent = null;
        HttpContent httpContent = new StringContent(content, Encoding.UTF8, HttpContentType);
        try
        {
            //Stopwatch sw = Stopwatch.StartNew();
            //sw.Stop();
            //logger?.LogError($"SOAP Action: {methodName}, Elapsed {sw.ElapsedMilliseconds} ms");
            HttpResponseMessage response = await client.PostAsync(Url, httpContent).ConfigureAwait(false);
            // 得到返回的结果，注意该结果是基于XML格式的，最后按照约定解析该XML格式中的内容即可。
            var result = await response.Content.ReadAsStreamAsync();
            rawContent = await response.Content.ReadAsStringAsync();
            // 解析内容
            //using var reader = new StringReader(result);
            using var xmlReader = XmlReader.Create(result);
            var doc = XDocument.Load(xmlReader);
            XmlNameTable nameTable = xmlReader.NameTable;
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace(NpAlia, EnvelopeNs);
            if (!string.IsNullOrEmpty(ResponseNamespace))
            {
                namespaceManager.AddNamespace(SoapResponse.RN_ALIAS, ResponseNamespace);
            }
            if (IsSoapFault(rawContent, EnvelopeNs))
            {
                //异常处理
                throw ParseSoapFault(doc, EnvelopeNs, Version, namespaceManager);
            }
            else
            {
                var innerXml = doc.XPathSelectElement($"//{NpAlia}:Body/{SoapResponse.RN_ALIAS}:{methodName}Response", namespaceManager)?.ToString();
                return new SoapResponse(content, rawContent, innerXml, namespaceManager, methodName);
            }
        }
        catch (Exception ex)
        {
            return new SoapResponse(content, rawContent, ex);
        }
    }

    internal static bool IsSoapFault(string content, string envelopeNs)
    {
        return content.Contains("<soap:Fault>") ||
               content.Contains("<soapenv:Fault>") ||
               content.Contains("<soap12:Fault>") ||
               content.Contains($"<Fault xmlns=\"{envelopeNs}\"");
    }

    internal static SoapFaultException ParseSoapFault(XDocument doc, string envelopeNs, SoapVersion version, IXmlNamespaceResolver resolver)
    {
        try
        {
            XNamespace ns = envelopeNs;
            var fault = doc.Descendants(ns + "Fault").FirstOrDefault();

            if (fault == null)
            {
                return new SoapFaultException(
                    "SOAP Fault (unable to parse details)",
                    "Unknown",
                    "Unparseable SOAP Fault");
            }

            if (version == SoapVersion.Soap12)
            {
                // SOAP 1.2 Fault结构
                var code = doc.XPathSelectElement($"//Code", resolver)?.ToString();
                var reason = doc.XPathSelectElement($"//Reason", resolver)?.ToString();
                var detail = doc.XPathSelectElement($"//Detail", resolver)?.ToString();

                return new SoapFaultException(
                    code,
                    reason,
                    detail);
            }
            else
            {
                // SOAP 1.1 Fault结构
                var faultCode = doc.XPathSelectElement($"//faultcode", resolver)?.Value;
                var faultString = doc.XPathSelectElement($"//faultstring", resolver)?.Value;
                var faultDetail = doc.XPathSelectElement($"//detail", resolver)?.Value;

                return new SoapFaultException(
                    faultCode,
                    faultString,
                    faultDetail);
            }
        }
        catch (Exception ex)
        {
            return new SoapFaultException(
                "SOAP Fault (parsing failed)",
                "ParseError",
                ex.Message);
        }
    }
}
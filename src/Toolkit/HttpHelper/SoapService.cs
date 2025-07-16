using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Net.Http;
using System.Xml;
using System.Xml.XPath;
using System.Linq;
using System.Threading;
using System.Diagnostics;
namespace MT.Toolkit.HttpHelper;

internal partial class SoapService : ISoapService//, IDisposable
{
    private const string SLASH = "/";
    private readonly IHttpClientFactory clientFactory;
    private readonly string? url;
    private readonly SoapVersion? version;
    private readonly Action<string> logAction;
    private readonly string? requestNamespace;
    private readonly string? responseNamespace;
    private readonly SoapServiceConfiguration? configuration;
    private bool disposedValue;
    #region 属性
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

    #endregion
    private readonly EmptyableSemaphero semaphore;
    class EmptyableSemaphero : IDisposable
    {
        private readonly SemaphoreSlim? semaphore;

        public EmptyableSemaphero(int count)
        {
            if (count > 0)
            {
                semaphore = new(count, count);
            }
        }

        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            return semaphore?.WaitAsync(cancellationToken) ?? Task.CompletedTask;
        }
        public void Release()
        {
            semaphore?.Release();
        }
        public void Dispose()
        {
            semaphore?.Dispose();
        }
    }
    #region 构造函数
    public SoapService(IHttpClientFactory clientFactory, SoapServiceConfiguration configuration, Action<string> logAction)
    {
        this.clientFactory = clientFactory;
        this.configuration = configuration;
        this.logAction = logAction;
        //this.httpClient = this.clientFactory.CreateClient(configuration.Name);
        semaphore = new(configuration.ConcurrencyLimit);
    }

    public SoapService(IHttpClientFactory clientFactory, string url, Action<string> logAction)
        : this(clientFactory, url, SoapVersion.Soap11, "http://tempuri.org/", logAction)
    {
    }

    public SoapService(IHttpClientFactory clientFactory
        , string url
        , string @namespace
        , Action<string> logAction)
        : this(clientFactory, url, SoapVersion.Soap11, @namespace, logAction)
    {
    }

    public SoapService(IHttpClientFactory clientFactory
        , string url
        , SoapVersion version
        , string @namespace
        , Action<string> logAction)
        : this(clientFactory, url, version, @namespace, @namespace, logAction)
    {
    }

    public SoapService(IHttpClientFactory clientFactory
        , string url
        , SoapVersion version
        , string requestNamespace
        , string responseNamespace
        , Action<string> logAction)
    {
        this.clientFactory = clientFactory;
        this.url = url;
        this.version = version;
        this.logAction = logAction;
        this.requestNamespace = requestNamespace.EndsWith(SLASH) ? requestNamespace : requestNamespace + '/';
        this.responseNamespace = responseNamespace.EndsWith(SLASH) ? requestNamespace : requestNamespace + '/';
        //this.httpClient = this.clientFactory.CreateClient(url);
        semaphore = new(SoapServiceConfiguration.DEFAULT_CONCURRENCY_LIMIT);
    }
    #endregion

    public Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        var client = clientFactory.CreateClient(configuration?.Name ?? Url);
        return SendAsync(client, methodName, args, cancellationToken);
    }

    public async Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        string content = BuildSoapRequest(methodName, args);
        string? rawContent = null;
        try
        {
            await semaphore.WaitAsync(cancellationToken);
            using HttpRequestMessage request = new(HttpMethod.Post, Url)
            {
                Content = new StringContent(content, Encoding.UTF8, HttpContentType)
            };
            if (Version == SoapVersion.Soap11)
            {
                request.Headers.Add("SOAPAction", $"{RequestNamespace}{methodName}");
            }
            var start = StopwatchHelper.GetTimestamp();
            using HttpResponseMessage response = await SendAsync(client, request, cancellationToken).ConfigureAwait(false);
            var elapsed = StopwatchHelper.GetElapsedTime(start);
            logAction($"{methodName}: 耗时 {elapsed.TotalMilliseconds}ms");
            // 得到返回的结果，注意该结果是基于XML格式的，最后按照约定解析该XML格式中的内容即可。
#if NET6_0_OR_GREATER
            rawContent = await response.Content.ReadAsStringAsync(cancellationToken);
#else
            rawContent = await response.Content.ReadAsStringAsync();
#endif
            // 解析内容
            var doc = XDocument.Parse(rawContent);
            XmlNamespaceManager resolver = new(new NameTable());
            resolver.AddNamespace(NpAlia, EnvelopeNs);
            resolver.AddNamespace(SoapResponse.RN_ALIAS, ResponseNamespace);
            if (IsSoapFault(rawContent, EnvelopeNs))
            {
                //异常处理
                throw ParseSoapFault(doc, EnvelopeNs, Version, resolver);
            }
            else
            {
                var innerXml = doc.XPathSelectElement($"//{NpAlia}:Body/{SoapResponse.RN_ALIAS}:{methodName}Response", resolver)?.ToString();
                return new SoapResponse(content, rawContent, innerXml, resolver, methodName);
            }
        }
        catch (Exception ex)
        {
            return new SoapResponse(content, rawContent, ex);
        }
    }

    /// <summary>
    /// 信号量阻塞的发送请求
    /// </summary>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<HttpResponseMessage> SendAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            return await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private string BuildSoapRequest(string methodName, Dictionary<string, object>? args)
    {
        StringBuilder contentString = new();
        if (args != null)
        {
            foreach (var item in args)
            {
                contentString.Append($"<{item.Key}><![CDATA[{FormatValue(item.Value)}]]></{item.Key}>");
            }
        }
        string content = $"""
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="{EnvelopeNs}" xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
       <soap:Body>
         <{methodName} xmlns="{RequestNamespace}">
             {contentString}
         </{methodName}>
     </soap:Body>
</soap:Envelope>
""";
        return content;
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

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                //httpClient.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

file class StopwatchHelper
{
    public static long GetTimestamp() => Stopwatch.GetTimestamp();
    public static TimeSpan GetElapsedTime(long startingTimestamp)
    {
#if NET8_0_OR_GREATER
        return Stopwatch.GetElapsedTime(startingTimestamp);
#else
        var end = Stopwatch.GetTimestamp();
        var tickFrequency = (double)(10000 * 1000 / Stopwatch.Frequency);
        var tick = (end - startingTimestamp) * tickFrequency;
        return new TimeSpan((long)tick);
#endif
    }
}
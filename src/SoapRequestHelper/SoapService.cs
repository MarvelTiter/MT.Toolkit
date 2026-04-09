using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace SoapRequestHelper;

internal partial class SoapService : ISoapService
{
    private readonly Action<string> logAction;
    private readonly SoapServiceConfiguration configuration;
    private bool disposedValue;
    private readonly IHttpClientPool httpClientPool;
    #region 属性
    private string Url => configuration.Url ?? throw new ArgumentNullException();
    private SoapVersion Version => configuration.Version ?? SoapVersion.Soap11;
    private string RequestNamespace => configuration.RequestNamespace ?? "http://tempuri.org/";
    private string ResponseNamespace => configuration.ResponseNamespace ?? "http://tempuri.org/";
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
    private readonly HttpRequestChannel<SoapRequest, HttpResponseMessage> requestChannel;

    private readonly record struct SoapRequest(
        HttpClient Client,
        string MethodName,
        HttpRequestMessage RequestMessage,
        TaskCompletionSource<HttpResponseMessage> CompletionSource,
        CancellationToken CancellationToken) : IHttpRequestChannelInput<HttpResponseMessage>;

    #region 构造函数
    public SoapService(SoapServiceConfiguration configuration, Action<string> logAction)
    {
        this.configuration = configuration;
        this.logAction = logAction;
        requestChannel = new(configuration.QueueCapacity, configuration.ConcurrencyLimit, ProcessSoapRequest);
        httpClientPool = configuration.HttpClientPool;
    }

    public SoapService(string name, Action<SoapServiceConfiguration> configAction, Action<string> logAction)
    {
        this.logAction = logAction;
        configuration = new SoapServiceConfiguration(name);
        configAction(configuration);
        requestChannel = new(configuration.QueueCapacity, configuration.ConcurrencyLimit, ProcessSoapRequest);
        httpClientPool = configuration.HttpClientPool ?? throw new ArgumentNullException("未配置连接池");
    }
    #endregion

    private async Task<HttpResponseMessage> ProcessSoapRequest(SoapRequest soapRequest)
    {
        var cancellationToken = soapRequest.CancellationToken;
        var client = soapRequest.Client;
        var start = StopwatchHelper.GetTimestamp();
        HttpResponseMessage response = await client.SendAsync(soapRequest.RequestMessage, cancellationToken).ConfigureAwait(false);
        var elapsed = StopwatchHelper.GetElapsedTime(start);
        response.EnsureSuccessStatusCode();
        logAction($"{soapRequest.MethodName}: 耗时 {elapsed.TotalMilliseconds}ms");
        return response;
    }

    public async ValueTask<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        //var client = clientFactory.CreateClient(configuration?.Name ?? Url);
        //using var client = new HttpClient();
        var client = await httpClientPool.GetAsync(new(configuration?.Name ?? Url, methodName, args), cancellationToken);
        try
        {
            var response = await SendAsync(client, methodName, args, cancellationToken);
            return response;
        }
        finally
        {
            httpClientPool.Return(client);
        }
    }

    //public async ValueTask<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    //{
    //    var client = clientFactory.CreateClient(configuration?.Name ?? Url);
    //    var response = await SendAsync(client, methodName, args, cancellationToken);
    //    return response;
    //}

    public async ValueTask<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<HttpResponseMessage>();
        string content = BuildSoapRequest(methodName, args);
        try
        {
            using HttpRequestMessage requestMessage = BuildRequestMessage(methodName, content);

            var request = new SoapRequest(client, methodName, requestMessage, tcs, cancellationToken);

            await requestChannel.WriteAsync(request);

            using var response = await tcs.Task;
            var start = StopwatchHelper.GetTimestamp();
            //var response = await client.SendAsync(requestMessage, cancellationToken);
            //// 处理响应
            //var elapsed = StopwatchHelper.GetElapsedTime(start);
            //logAction($"methodName: {elapsed}");
            var soapResponse = await HandleResponse(response, methodName, content, cancellationToken);
            return soapResponse;
        }
        catch (OperationCanceledException)
        {
            return new SoapResponse(content, null, new SoapFaultException("Request canceled", "Canceled", "The request was canceled"));
        }
        catch (Exception ex)
        {
            return new SoapResponse(content, null, ex);
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

    private HttpRequestMessage BuildRequestMessage(string methodName, string content)
    {
        HttpRequestMessage requestMessage = new(HttpMethod.Post, Url)
        {
            Content = new StringContent(content, Encoding.UTF8, HttpContentType)
        };
        if (Version == SoapVersion.Soap11)
        {
            requestMessage.Headers.Add("SOAPAction", $"{RequestNamespace}{methodName}");
        }

        return requestMessage;
    }

    private async Task<SoapResponse> HandleResponse(HttpResponseMessage response, string methodName, string content, CancellationToken cancellationToken)
    {
        string? rawContent = null;
        try
        {
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

    public async ValueTask DisposeAsync()
    {
        if (disposedValue) return;
        await requestChannel.DisposeAsync();
        await httpClientPool.DisposeAsync();
        disposedValue = true;
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
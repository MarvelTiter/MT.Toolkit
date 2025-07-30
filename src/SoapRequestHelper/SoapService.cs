using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
namespace SoapRequestHelper;

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
    private readonly SoapRequestChannel requestChannel;

    private record SoapRequest(
        HttpClient Client,
        string MethodName,
        Dictionary<string, object>? Args,
        TaskCompletionSource<SoapResponse> CompletionSource,
        CancellationToken CancellationToken);

    private class SoapRequestChannel : IAsyncDisposable
    {
        private readonly Channel<SoapRequest>? channel;
        private readonly Task[] workerTasks = [];
        private readonly Func<SoapRequest, Task<SoapResponse>> handler;
        private readonly Func<SoapRequest, Task> writer;
        private readonly CancellationTokenSource cts = new();
        public SoapRequestChannel(int capacity, Func<SoapRequest, Task<SoapResponse>> handler)
        {
            if (capacity > 0)
            {
                channel = Channel.CreateBounded<SoapRequest>(new BoundedChannelOptions(capacity)
                {
                    FullMode = BoundedChannelFullMode.Wait,
                    SingleReader = false,
                    SingleWriter = false
                });

                // 启动工作线程
                workerTasks = new Task[capacity];
                for (int i = 0; i < capacity; i++)
                {
                    workerTasks[i] = Task.Run(ProcessRequestsAsync);
                }
                writer = WriteIntoChannelAsync;
            }
            else
            {
                writer = WriteDirectlyAsync;
            }

            this.handler = handler;
        }
        private async Task ProcessRequestsAsync()
        {
            if (channel == null) return;
            await foreach (var request in channel.Reader.ReadAllAsync(cts.Token))
            {
                try
                {
                    var response = await handler(request);
                    request.CompletionSource.SetResult(response);
                }
                catch (Exception ex)
                {
                    request.CompletionSource.SetException(ex);
                }
            }
        }

        private async Task WriteIntoChannelAsync(SoapRequest request)
        {
            if (channel == null) return;
            await channel.Writer.WriteAsync(request, request.CancellationToken);
        }

        private Task WriteDirectlyAsync(SoapRequest request)
        {
            // 使用ConfigureAwait(false)避免同步上下文问题
            return HandleRequestAsync(request);

            async Task HandleRequestAsync(SoapRequest req)
            {
                try
                {
                    var response = await handler(req).ConfigureAwait(false);
                    req.CompletionSource.TrySetResult(response);
                }
                catch (Exception ex)
                {
                    req.CompletionSource.TrySetException(ex);
                }
            }
        }

        public Task WriteAsync(SoapRequest request) => writer(request);

        public async ValueTask DisposeAsync()
        {
            if (channel != null)
            {
                channel.Writer.Complete();
                await Task.WhenAll(workerTasks);
                foreach (var task in workerTasks)
                {
                    task.Dispose();
                }
            }
            cts.Cancel();
            cts.Dispose();
        }
    }

    #region 构造函数
    public SoapService(IHttpClientFactory clientFactory, SoapServiceConfiguration configuration, Action<string> logAction)
    {
        this.clientFactory = clientFactory;
        this.configuration = configuration;
        this.logAction = logAction;
        //this.httpClient = this.clientFactory.CreateClient(configuration.Name);
        requestChannel = new SoapRequestChannel(configuration.ConcurrencyLimit, ProcessSoapRequest);
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
        requestChannel = new SoapRequestChannel(SoapServiceConfiguration.DEFAULT_CONCURRENCY_LIMIT, ProcessSoapRequest);
    }
    #endregion

    private async Task<SoapResponse> ProcessSoapRequest(SoapRequest soapRequest)
    {
        var methodName = soapRequest.MethodName;
        var args = soapRequest.Args;
        var cancellationToken = soapRequest.CancellationToken;
        var client = soapRequest.Client;
        string content = BuildSoapRequest(methodName, args);
        string? rawContent = null;
        try
        {
            using HttpRequestMessage request = new(HttpMethod.Post, Url)
            {
                Content = new StringContent(content, Encoding.UTF8, HttpContentType)
            };
            if (Version == SoapVersion.Soap11)
            {
                request.Headers.Add("SOAPAction", $"{RequestNamespace}{methodName}");
            }
            var start = StopwatchHelper.GetTimestamp();
            using HttpResponseMessage response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
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

    public Task<SoapResponse> SendAsync(string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        var client = clientFactory.CreateClient(configuration?.Name ?? Url);
        return SendAsync(client, methodName, args, cancellationToken);
    }

    public async Task<SoapResponse> SendAsync(HttpClient client, string methodName, Dictionary<string, object>? args = null, CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<SoapResponse>();
        var request = new SoapRequest(client, methodName, args, tcs, cancellationToken);

        await requestChannel.WriteAsync(request);
        return await tcs.Task;
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

    public async ValueTask DisposeAsync()
    {
        if (disposedValue) return;
        await requestChannel.DisposeAsync();
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
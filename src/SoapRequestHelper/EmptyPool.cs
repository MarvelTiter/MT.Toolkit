namespace SoapRequestHelper;

internal class EmptyPool(IHttpClientFactory clientFactory) : IHttpClientPool
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<HttpClient> GetAsync(HttpClientCreationContext context, CancellationToken cancellationToken = default)
    {
        return ValueTask.FromResult(clientFactory.CreateClient(context.ConfigurationName));
    }

    public void Return(HttpClient _)
    {

    }
}




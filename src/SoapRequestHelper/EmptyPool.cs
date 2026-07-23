namespace SoapRequestHelper;

internal class EmptyPool(IHttpClientFactory clientFactory) : IHttpClientPool
{
    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public Task<HttpClient> GetAsync(HttpClientCreationContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(clientFactory.CreateClient(context.ConfigurationName));
    }

    public void Return(HttpClient _)
    {

    }
}




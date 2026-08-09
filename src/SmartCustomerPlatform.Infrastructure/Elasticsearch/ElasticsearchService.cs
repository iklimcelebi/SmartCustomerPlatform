using Elastic.Clients.Elasticsearch;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;

namespace SmartCustomerPlatform.Infrastructure.Elasticsearch;

public class ElasticsearchService : IElasticsearchService
{
    private readonly ElasticsearchClient _client;

    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task IndexAsync<T>(
        string indexName,
        string id,
        T document,
        CancellationToken cancellationToken = default)
    {
        await _client.IndexAsync(
            document,
            request => request
                .Index(indexName)
                .Id(id),
            cancellationToken);
    }

    public async Task UpdateAsync<T>(
        string indexName,
        string id,
        T partialDocument,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.UpdateAsync<T, T>(
            new UpdateRequest<T, T>(indexName, id)
            {
                Doc = partialDocument
            },
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch update failed: {response.DebugInformation}");
        }
    }
}
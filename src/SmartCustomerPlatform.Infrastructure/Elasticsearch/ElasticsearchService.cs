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
        var response = await _client.IndexAsync(
            document,
            request => request
                .Index(indexName)
                .Id(id),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch index failed: {response.DebugInformation}");
        }
    }

    public async Task UpdateAsync<T>(
        string indexName,
        string id,
        T partialDocument,
        CancellationToken cancellationToken = default)
    {
        var response = await _client.UpdateAsync<T, T>(
            indexName,
            id,
            request => request
                .Doc(partialDocument),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch update failed: {response.DebugInformation}");
        }
    }
}
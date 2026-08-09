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

    public async Task CreateTicketIndexAsync(
        CancellationToken cancellationToken = default)
    {
        const string indexName = "tickets-v1";

        var existsResponse = await _client.Indices.ExistsAsync(
            indexName,
            cancellationToken);

        if (existsResponse.Exists)
        {
            return;
        }

        var response = await _client.Indices.CreateAsync(
            indexName,
            descriptor => descriptor
                .Mappings(mappings => mappings
                    .Properties<TicketDocument>(properties => properties
                        .Keyword(x => x.TicketId)
                        .Keyword(x => x.TicketNumber)
                        .Keyword(x => x.CustomerId)
                        .Keyword(x => x.DepartmentId)
                        .Keyword(x => x.CategoryId)
                        .Keyword(x => x.SubCategoryId)
                        .Text(x => x.Subject)
                        .Keyword(x => x.Priority)
                        .Keyword(x => x.Status)
                        .Date(x => x.OccurredOn)
                        .Keyword(x => x.AssignedUserId)
                    )),
            cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch index creation failed: {response.DebugInformation}");
        }
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
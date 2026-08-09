namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public interface IElasticsearchService
{
    Task CreateTicketIndexAsync(
        CancellationToken cancellationToken = default);

    Task IndexAsync<T>(
        string indexName,
        string id,
        T document,
        CancellationToken cancellationToken = default);

    Task UpdateAsync<T>(
        string indexName,
        string id,
        T partialDocument,
        CancellationToken cancellationToken = default);
}

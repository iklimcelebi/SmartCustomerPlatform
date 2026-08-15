namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public interface IElasticsearchService
{
    Task CreateTicketIndexAsync(
        CancellationToken cancellationToken = default);

    Task RecreateTicketIndexAsync(
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

    Task<IReadOnlyList<TicketSearchResult>>
        SearchTicketsAsync(
            string? searchTerm = null,
            string? status = null,
            string? priority = null,
            Guid? departmentId = null,
            Guid? categoryId = null,
            Guid? assignedUserId = null,
            bool? slaBreached = null,
            CancellationToken cancellationToken = default);
}
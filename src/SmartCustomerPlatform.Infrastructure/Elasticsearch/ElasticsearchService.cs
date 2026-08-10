using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;

namespace SmartCustomerPlatform.Infrastructure.Elasticsearch;

public class ElasticsearchService : IElasticsearchService
{
    private const string TicketIndex = "tickets-v1";

    private readonly ElasticsearchClient _client;

    public ElasticsearchService(ElasticsearchClient client)
    {
        _client = client;
    }

    // ============================================================
    // CREATE INDEX
    // ============================================================

    public async Task CreateTicketIndexAsync(
        CancellationToken cancellationToken = default)
    {
        var existsResponse =
            await _client.Indices.ExistsAsync(
                TicketIndex,
                cancellationToken);

        if (existsResponse.Exists)
        {
            return;
        }

        var response =
            await _client.Indices.CreateAsync(
                TicketIndex,
                descriptor => descriptor
                    .Mappings(mappings => mappings
                        .Properties<TicketDocument>(properties => properties

                            // Basic fields
                            .Keyword(x => x.TicketId)
                            .Keyword(x => x.TicketNumber)

                            .Keyword(x => x.CustomerId)
                            .Text(x => x.CustomerName)

                            .Keyword(x => x.DepartmentId)
                            .Keyword(x => x.CategoryId)
                            .Keyword(x => x.SubCategoryId)

                            .Text(x => x.Subject)
                            .Text(x => x.Description)

                            .Keyword(x => x.Priority)
                            .Keyword(x => x.Status)

                            .Date(x => x.OccurredOn)

                            .Keyword(x => x.AssignedUserId)

                            // SLA
                            .Date(x => x.SlaStartedAt)
                            .Date(x => x.SlaResponseDueAt)
                            .Date(x => x.SlaResolutionDueAt)
                            .Boolean(x => x.IsSlaPaused)
                            .Date(x => x.SlaPausedAt)
                        )
                    ),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch index creation failed: " +
                response.DebugInformation);
        }
    }

    // ============================================================
    // RECREATE INDEX
    // ============================================================

    public async Task RecreateTicketIndexAsync(
        CancellationToken cancellationToken = default)
    {
        var existsResponse =
            await _client.Indices.ExistsAsync(
                TicketIndex,
                cancellationToken);

        if (existsResponse.Exists)
        {
            var deleteResponse =
                await _client.Indices.DeleteAsync(
                    TicketIndex,
                    cancellationToken);

            if (!deleteResponse.IsValidResponse)
            {
                throw new InvalidOperationException(
                    $"Elasticsearch index deletion failed: " +
                    deleteResponse.DebugInformation);
            }
        }

        await CreateTicketIndexAsync(cancellationToken);
    }

    // ============================================================
    // INDEX DOCUMENT
    // ============================================================

    public async Task IndexAsync<T>(
        string indexName,
        string id,
        T document,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _client.IndexAsync(
                document,
                request => request
                    .Index(indexName)
                    .Id(id),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch index failed: " +
                response.DebugInformation);
        }
    }

    // ============================================================
    // UPDATE DOCUMENT
    // ============================================================

    public async Task UpdateAsync<T>(
        string indexName,
        string id,
        T partialDocument,
        CancellationToken cancellationToken = default)
    {
        var response =
            await _client.UpdateAsync<T, T>(
                indexName,
                id,
                request => request
                    .Doc(partialDocument),
                cancellationToken);

        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch update failed: " +
                response.DebugInformation);
        }
    }

    // ============================================================
    // TICKET SEARCH
    // ============================================================

    public async Task<IReadOnlyList<TicketSearchResult>>
        SearchTicketsAsync(
            string? searchTerm = null,
            string? status = null,
            string? priority = null,
            Guid? departmentId = null,
            Guid? categoryId = null,
            bool? slaBreached = null,
            CancellationToken cancellationToken = default)
    {
        var queries = new List<Query>();

        // ========================================================
        // FREE TEXT SEARCH
        // ========================================================

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            queries.Add(
                new MultiMatchQuery
                {
                    Query = searchTerm,

                    Fields = new[]
                    {
                        new Field("ticketNumber"),
                        new Field("subject"),
                        new Field("description"),
                        new Field("customerName")
                    }
                });
        }

        // ========================================================
        // STATUS FILTER
        // ========================================================

        if (!string.IsNullOrWhiteSpace(status))
        {
            queries.Add(
                new TermQuery
                {
                    Field = new Field("status"),
                    Value = status
                });
        }

        // ========================================================
        // PRIORITY FILTER
        // ========================================================

        if (!string.IsNullOrWhiteSpace(priority))
        {
            queries.Add(
                new TermQuery
                {
                    Field = new Field("priority"),
                    Value = priority
                });
        }

        // ========================================================
        // DEPARTMENT FILTER
        // ========================================================

        if (departmentId.HasValue)
        {
            queries.Add(
                new TermQuery
                {
                    Field = new Field("departmentId"),
                    Value = departmentId.Value.ToString()
                });
        }

        // ========================================================
        // CATEGORY FILTER
        // ========================================================

        if (categoryId.HasValue)
        {
            queries.Add(
                new TermQuery
                {
                    Field = new Field("categoryId"),
                    Value = categoryId.Value.ToString()
                });
        }

        // ========================================================
        // SLA FILTER
        // ========================================================

        if (slaBreached.HasValue)
        {
            var now = DateTime.UtcNow;

            if (slaBreached.Value)
            {
                // SLA ihlali:
                // Response deadline geçmiş
                // VEYA
                // Resolution deadline geçmiş

                queries.Add(
                    new BoolQuery
                    {
                        Should = new List<Query>
                        {
                            new DateRangeQuery
                            {
                                Field = new Field(
                                    "slaResponseDueAt"),

                                Lt = now
                            },

                            new DateRangeQuery
                            {
                                Field = new Field(
                                    "slaResolutionDueAt"),

                                Lt = now
                            }
                        },

                        MinimumShouldMatch = 1
                    });
            }
            else
            {
                // SLA ihlali olmayanlar

                queries.Add(
                    new BoolQuery
                    {
                        MustNot = new List<Query>
                        {
                            new DateRangeQuery
                            {
                                Field = new Field(
                                    "slaResponseDueAt"),

                                Lt = now
                            },

                            new DateRangeQuery
                            {
                                Field = new Field(
                                    "slaResolutionDueAt"),

                                Lt = now
                            }
                        }
                    });
            }
        }

        // ========================================================
        // FINAL QUERY
        // ========================================================

        Query finalQuery;

        if (queries.Count == 0)
        {
            finalQuery =
                new MatchAllQuery();
        }
        else
        {
            finalQuery =
                new BoolQuery
                {
                    Must = queries
                };
        }

        // ========================================================
        // EXECUTE SEARCH
        // ========================================================

        var response =
            await _client.SearchAsync<TicketDocument>(
                search => search
                    .Indices(TicketIndex)
                    .Size(100)
                    .Query(finalQuery),
                cancellationToken);
        if (!response.IsValidResponse)
        {
            throw new InvalidOperationException(
                $"Elasticsearch search failed: " +
                response.DebugInformation);
        }

        // ========================================================
        // MAP RESULT
        // ========================================================

        var nowUtc = DateTime.UtcNow;

        return response.Documents
            .Select(x => new TicketSearchResult
            {
                TicketId = x.TicketId,

                TicketNumber =
                    x.TicketNumber,

                CustomerId =
                    x.CustomerId,

                CustomerName =
                    x.CustomerName,

                DepartmentId =
                    x.DepartmentId,

                CategoryId =
                    x.CategoryId,

                SubCategoryId =
                    x.SubCategoryId,

                Subject =
                    x.Subject,

                Description =
                    x.Description,

                Priority =
                    x.Priority,

                Status =
                    x.Status,

                OccurredOn =
                    x.OccurredOn,

                AssignedUserId =
                    x.AssignedUserId,

                SlaStartedAt =
                    x.SlaStartedAt,

                SlaResponseDueAt =
                    x.SlaResponseDueAt,

                SlaResolutionDueAt =
                    x.SlaResolutionDueAt,

                IsSlaPaused =
                    x.IsSlaPaused,

                SlaPausedAt =
                    x.SlaPausedAt,

                TotalSlaPausedDuration =
                    x.TotalSlaPausedDuration,

                IsSlaBreached =
                    x.SlaResponseDueAt < nowUtc
                    ||
                    x.SlaResolutionDueAt < nowUtc
            })
            .ToList();
    }
}
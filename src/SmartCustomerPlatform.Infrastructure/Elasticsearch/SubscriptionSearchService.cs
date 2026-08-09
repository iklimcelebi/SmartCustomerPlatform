using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

namespace SmartCustomerPlatform.Infrastructure.Elasticsearch;

public class SubscriptionSearchService : ISubscriptionSearchService
{
    private readonly ElasticsearchClient _client;
    private const string IndexName = "subscriptions-v1";

    public SubscriptionSearchService(
        ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<List<SubscriptionSearchResultDto>> SearchAsync(
        Guid? subscriptionId,
        Guid? packageId,
        string? status,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        CancellationToken cancellationToken = default)
    {
        var filters = new List<Query>();

        if (subscriptionId.HasValue)
        {
            filters.Add(new Query
            {
                Term = new TermQuery
                {
                    Field = Infer.Field<SubscriptionSearchResultDto>(
                        x => x.Id),
                    Value = subscriptionId.Value.ToString()
                }
            });
        }

        if (packageId.HasValue)
        {
            filters.Add(new Query
            {
                Term = new TermQuery
                {
                    Field = Infer.Field<SubscriptionSearchResultDto>(
                        x => x.PackageId),
                    Value = packageId.Value.ToString()
                }
            });
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            filters.Add(new Query
            {
                Term = new TermQuery
                {
                    Field = Infer.Field<SubscriptionSearchResultDto>(
                        x => x.Status),
                    Value = status
                }
            });
        }

        Query query = filters.Count == 0
            ? new Query
            {
                MatchAll = new MatchAllQuery()
            }
            : new Query
            {
                Bool = new BoolQuery
                {
                    Filter = filters
                }
            };

        var request =
            new SearchRequest<SubscriptionSearchResultDto>(
                IndexName)
            {
                Size = 100,
                Query = query
            };

        var response =
            await _client.SearchAsync<SubscriptionSearchResultDto>(
                request,
                cancellationToken);

        if (!response.IsValidResponse)
        {
            return new List<SubscriptionSearchResultDto>();
        }

        return response.Documents.ToList();
    }

    public async Task<SubscriptionDashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default)
    {
        var request =
            new SearchRequest<SubscriptionSearchResultDto>(
                IndexName)
            {
                Size = 1000,
                Query = new Query
                {
                    MatchAll = new MatchAllQuery()
                }
            };

        var response =
            await _client.SearchAsync<SubscriptionSearchResultDto>(
                request,
                cancellationToken);

        if (!response.IsValidResponse)
        {
            return new SubscriptionDashboardDto();
        }

        var documents =
            response.Documents.ToList();

        var activeSubscriptions =
            documents
                .Where(x => x.IsActive)
                .ToList();

        var activeSubscriptionCount =
            activeSubscriptions.Count;

        var thirtyDaysAgo =
            DateTime.UtcNow.AddDays(-30);

        var cancelledLast30DaysCount =
            documents.Count(
                x =>
                    string.Equals(
                        x.Status,
                        "Cancelled",
                        StringComparison.OrdinalIgnoreCase) &&
                    x.EndDate.HasValue &&
                    x.EndDate.Value >= thirtyDaysAgo);

        var activeSubscriptionsByPackage =
            activeSubscriptions
                .GroupBy(x => x.PackageId)
                .Select(group =>
                    new ActiveSubscriptionByPackageDto
                    {
                        PackageId = group.Key,
                        Count = group.Count()
                    })
                .OrderByDescending(x => x.Count)
                .ToList();

        return new SubscriptionDashboardDto
        {
            ActiveSubscriptionCount =
                activeSubscriptionCount,

            CancelledLast30DaysCount =
                cancelledLast30DaysCount,

            ActiveSubscriptionsByPackage =
                activeSubscriptionsByPackage
        };
    }
}
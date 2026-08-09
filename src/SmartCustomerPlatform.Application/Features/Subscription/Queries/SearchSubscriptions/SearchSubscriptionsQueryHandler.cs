using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

public class SearchSubscriptionsQueryHandler
    : IRequestHandler<SearchSubscriptionsQuery, List<SubscriptionSearchResultDto>>
{
    private readonly ISubscriptionSearchService _subscriptionSearchService;

    public SearchSubscriptionsQueryHandler(
        ISubscriptionSearchService subscriptionSearchService)
    {
        _subscriptionSearchService = subscriptionSearchService;
    }

    public async Task<List<SubscriptionSearchResultDto>> Handle(
        SearchSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _subscriptionSearchService.SearchAsync(
            request.SubscriptionId,
            request.PackageId,
            request.Status,
            request.StartDateFrom,
            request.StartDateTo,
            cancellationToken);
    }
}
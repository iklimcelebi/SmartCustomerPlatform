using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;

public class GetSubscriptionDashboardQueryHandler
    : IRequestHandler<GetSubscriptionDashboardQuery, SubscriptionDashboardDto>
{
    private readonly ISubscriptionSearchService _subscriptionSearchService;

    public GetSubscriptionDashboardQueryHandler(
        ISubscriptionSearchService subscriptionSearchService)
    {
        _subscriptionSearchService = subscriptionSearchService;
    }

    public async Task<SubscriptionDashboardDto> Handle(
        GetSubscriptionDashboardQuery request,
        CancellationToken cancellationToken)
    {
        return await _subscriptionSearchService.GetDashboardAsync(
            cancellationToken);
    }
}
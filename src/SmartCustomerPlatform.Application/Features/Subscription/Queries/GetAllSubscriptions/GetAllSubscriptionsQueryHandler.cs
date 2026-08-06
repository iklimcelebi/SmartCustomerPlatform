using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetAllSubscriptions;

public class GetAllSubscriptionsQueryHandler
    : IRequestHandler<GetAllSubscriptionsQuery, List<SubscriptionDto>>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetAllSubscriptionsQueryHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<List<SubscriptionDto>> Handle(
        GetAllSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var subscriptions = await _subscriptionRepository.GetAllAsync(
            cancellationToken);

        return subscriptions.Select(subscription => new SubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            PackageId = subscription.PackageId,
            MonthlyPrice = subscription.MonthlyPrice,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            CampaignId = subscription.CampaignId,
            DiscountedPrice = subscription.DiscountedPrice,
            IsActive = subscription.IsActive
        }).ToList();
    }
}
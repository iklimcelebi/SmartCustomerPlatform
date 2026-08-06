using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;

public class GetSubscriptionByIdQueryHandler
    : IRequestHandler<GetSubscriptionByIdQuery, SubscriptionDto?>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public GetSubscriptionByIdQueryHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<SubscriptionDto?> Handle(
        GetSubscriptionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return null;

        return new SubscriptionDto
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
        };
    }
}
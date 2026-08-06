using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.UpdateSubscription;

public class UpdateSubscriptionCommandHandler
    : IRequestHandler<UpdateSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public UpdateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(
        UpdateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return false;

        subscription.UpdateDetails(
            request.MonthlyPrice,
            request.StartDate,
            request.EndDate,
            request.CampaignId,
            request.DiscountedPrice,
            request.IsActive);

        await _subscriptionRepository.UpdateAsync(
            subscription,
            cancellationToken);

        return true;
    }
}
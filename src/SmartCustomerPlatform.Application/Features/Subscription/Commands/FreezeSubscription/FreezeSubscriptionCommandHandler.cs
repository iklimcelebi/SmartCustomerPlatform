using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.FreezeSubscription;

public class FreezeSubscriptionCommandHandler
    : IRequestHandler<FreezeSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public FreezeSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(
        FreezeSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return false;

        subscription.Freeze();

        await _subscriptionRepository.UpdateAsync(
            subscription,
            cancellationToken);

        return true;
    }
}
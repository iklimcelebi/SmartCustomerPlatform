using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.ActivateSubscription;

public class ActivateSubscriptionCommandHandler
    : IRequestHandler<ActivateSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public ActivateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(
        ActivateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return false;

        subscription.Activate();

        await _subscriptionRepository.UpdateAsync(
            subscription,
            cancellationToken);

        return true;
    }
}
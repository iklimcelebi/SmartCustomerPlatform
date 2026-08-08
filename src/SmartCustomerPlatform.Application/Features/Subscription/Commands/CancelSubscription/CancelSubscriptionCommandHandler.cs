using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.CancelSubscription;

public class CancelSubscriptionCommandHandler
    : IRequestHandler<CancelSubscriptionCommand, bool>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CancelSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(
        CancelSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return false;

        subscription.Cancel();

        await _subscriptionRepository.UpdateAsync(
            subscription,
            cancellationToken);

        return true;
    }
}
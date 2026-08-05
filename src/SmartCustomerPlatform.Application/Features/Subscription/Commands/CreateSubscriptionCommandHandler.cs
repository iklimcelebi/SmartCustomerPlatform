using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;
using SubscriptionEntity = SmartCustomerPlatform.Domain.Entities.Subscription;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CreateSubscriptionCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Guid> Handle(
        CreateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = new SubscriptionEntity(
            request.CustomerId,
            request.PackageId,
            request.MonthlyPrice,
            request.StartDate,
            request.CampaignId,
            request.DiscountedPrice);

        await _subscriptionRepository.AddAsync(
            subscription,
            cancellationToken);

        return subscription.Id;
    }
}
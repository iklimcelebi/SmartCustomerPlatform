using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Commands.CreateSubscription;

public class CreateSubscriptionCommandHandler
    : IRequestHandler<CreateSubscriptionCommand, Guid>
{
    private readonly IGenericRepository<Subscription> _subscriptionRepository;

    public CreateSubscriptionCommandHandler(
        IGenericRepository<Subscription> subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Guid> Handle(
        CreateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = new Subscription
        {
            SubscriptionNumber = request.SubscriptionNumber,
            PackageName = request.PackageName,
            MonthlyFee = request.MonthlyFee,
            DiscountedPrice = request.DiscountedPrice,

            UsedQuota = request.UsedQuota,
            RemainingQuota = request.RemainingQuota,
            TotalQuota = request.TotalQuota,

            StartDate = request.StartDate,
            EndDate = request.EndDate,

            IsAutoRenew = request.IsAutoRenew,
            Status = request.Status,

            CustomerId = request.CustomerId,
            PackageId = request.PackageId,
            CampaignId = request.CampaignId
        };

        subscription.AddCreatedEvent();

        await _subscriptionRepository.AddAsync(subscription);
        await _subscriptionRepository.SaveChangesAsync();
        
        return subscription.Id;
    }
}


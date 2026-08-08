using MediatR;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Events;

public class SubscriptionCreatedEventHandler
    : INotificationHandler<SubscriptionCreatedEvent>
{
    public Task Handle(
        SubscriptionCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DomainEvent] Subscription created: " +
            $"SubscriptionId={notification.SubscriptionId}, " +
            $"CustomerId={notification.CustomerId}, " +
            $"PackageId={notification.PackageId}, " +
            $"CampaignId={notification.CampaignId}");

        return Task.CompletedTask;
    }
}

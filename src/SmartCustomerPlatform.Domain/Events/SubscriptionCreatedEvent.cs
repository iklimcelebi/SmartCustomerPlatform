using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public record SubscriptionCreatedEvent(
    Guid SubscriptionId,
    Guid CustomerId,
    Guid PackageId,
    Guid? CampaignId,
    decimal MonthlyPrice,
    decimal DiscountedPrice,
    DateTime StartDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

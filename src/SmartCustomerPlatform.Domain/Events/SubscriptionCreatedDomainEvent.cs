using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public sealed record SubscriptionCreatedDomainEvent(
    Guid EventId,
    Guid AggregateId,
    DateTime OccurredAtUtc,
    string CorrelationId,
    string? CausationId,
    string? PerformedBy)
    : DomainEventBase(
        EventId,
        AggregateId,
        OccurredAtUtc,
        CorrelationId,
        CausationId,
        PerformedBy)
{
    public Guid CustomerId { get; init; }

    public Guid PackageId { get; init; }

    public Guid? CampaignId { get; init; }

    public decimal MonthlyPrice { get; init; }

    public decimal DiscountedPrice { get; init; }

    public DateTime StartDate { get; init; }
}
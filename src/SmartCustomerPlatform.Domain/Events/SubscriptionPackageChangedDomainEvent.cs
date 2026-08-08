using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public sealed record SubscriptionPackageChangedDomainEvent(
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
    public Guid PackageId { get; init; }

    public decimal MonthlyPrice { get; init; }
}
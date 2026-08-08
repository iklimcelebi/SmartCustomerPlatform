using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public sealed record SubscriptionFrozenDomainEvent(
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
        PerformedBy);
namespace SmartCustomerPlatform.Domain.Common;

public abstract record DomainEventBase(
    Guid EventId,
    Guid AggregateId,
    DateTime OccurredAtUtc,
    string CorrelationId,
    string? CausationId,
    string? PerformedBy) : IDomainEvent
{
    public string EventType => GetType().Name;
}
namespace SmartCustomerPlatform.Domain.Common;

public interface IDomainEvent
{
    Guid EventId { get; }

    Guid AggregateId { get; }

    DateTime OccurredAtUtc { get; }

    string EventType { get; }

    string CorrelationId { get; }

    string? CausationId { get; }

    string? PerformedBy { get; }
}
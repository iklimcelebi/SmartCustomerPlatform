using MediatR;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketPriorityChangedEvent(
    Guid TicketId,
    TicketPriority OldPriority,
    TicketPriority NewPriority
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

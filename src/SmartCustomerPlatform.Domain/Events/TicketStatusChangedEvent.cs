using MediatR;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketStatusChangedEvent(
    Guid TicketId,
    TicketStatus OldStatus,
    TicketStatus NewStatus
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

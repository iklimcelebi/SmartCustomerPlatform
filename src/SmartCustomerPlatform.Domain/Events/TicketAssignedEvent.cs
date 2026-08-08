using MediatR;
using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketAssignedEvent(
    Guid TicketId,
    Guid AssignedUserId
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
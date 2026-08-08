using MediatR;
using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketReopenedEvent(
    Guid TicketId
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

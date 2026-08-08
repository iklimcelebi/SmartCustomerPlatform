using MediatR;
using SmartCustomerPlatform.Domain.Common;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketTransferredEvent(
    Guid TicketId,
    Guid FromDepartmentId,
    Guid ToDepartmentId
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

using MediatR;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketCreatedEvent(
    Guid TicketId,
    string TicketNumber,
    Guid CustomerId,
    Guid DepartmentId,
    Guid CategoryId,
    Guid? SubCategoryId,
    string Subject,
    TicketPriority Priority
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}


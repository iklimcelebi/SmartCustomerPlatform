using MediatR;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketCreatedEvent(
    Guid TicketId,
    string TicketNumber,
    Guid CustomerId,
    string CustomerName,
    Guid DepartmentId,
    Guid CategoryId,
    Guid? SubCategoryId,
    string Subject,
    string Description,
    TicketPriority Priority,
    DateTime SlaStartedAt,
    DateTime SlaResponseDueAt,
    DateTime SlaResolutionDueAt
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
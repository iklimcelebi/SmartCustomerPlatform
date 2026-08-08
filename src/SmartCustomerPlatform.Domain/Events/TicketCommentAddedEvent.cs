using MediatR;
using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Events;

public record TicketCommentAddedEvent(
    Guid TicketId,
    Guid CommentId,
    CommentType CommentType
) : IDomainEvent, INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

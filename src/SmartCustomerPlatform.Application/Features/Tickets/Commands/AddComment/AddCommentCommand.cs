using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.AddComment;

public record AddCommentCommand(
    Guid TicketId,
    string Content,
    string Author,
    CommentType Type
) : IRequest<Guid>;
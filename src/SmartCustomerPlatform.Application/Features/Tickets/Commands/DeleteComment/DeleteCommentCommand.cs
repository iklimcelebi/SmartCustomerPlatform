using MediatR;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.DeleteComment;

public record DeleteCommentCommand(
    Guid CommentId
) : IRequest;
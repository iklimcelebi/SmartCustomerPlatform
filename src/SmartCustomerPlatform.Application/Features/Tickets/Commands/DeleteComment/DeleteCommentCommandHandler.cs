using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.DeleteComment;

public class DeleteCommentCommandHandler
    : IRequestHandler<DeleteCommentCommand>
{
    private readonly ICommentRepository _commentRepository;

    public DeleteCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task Handle(
        DeleteCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment =
            await _commentRepository.GetByIdIncludingDeletedAsync(
                request.CommentId);

        if (comment is null)
            throw new KeyNotFoundException("Comment not found.");

        if (comment.IsDeleted)
            return;

        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await _commentRepository.SaveChangesAsync();
    }
}
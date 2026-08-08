using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.AddComment;

public class AddCommentCommandHandler
    : IRequestHandler<AddCommentCommand, Guid>
{
    private readonly ICommentRepository _commentRepository;

    public AddCommentCommandHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<Guid> Handle(
        AddCommentCommand request,
        CancellationToken cancellationToken)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TicketId = request.TicketId,
            Content = request.Content,
            Author = request.Author,
            Type = request.Type
        };

        await _commentRepository.AddAsync(comment);

        return comment.Id;
    }
}
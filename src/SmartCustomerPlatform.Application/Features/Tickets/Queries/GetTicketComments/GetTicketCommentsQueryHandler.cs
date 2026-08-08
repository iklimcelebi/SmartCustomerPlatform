using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketComments;

public class GetTicketCommentsQueryHandler
    : IRequestHandler<
        GetTicketCommentsQuery,
        IReadOnlyList<Comment>>
{
    private readonly ICommentRepository _commentRepository;

    public GetTicketCommentsQueryHandler(
        ICommentRepository commentRepository)
    {
        _commentRepository = commentRepository;
    }

    public async Task<IReadOnlyList<Comment>> Handle(
        GetTicketCommentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _commentRepository.GetByTicketIdAsync(
            request.TicketId);
    }
}

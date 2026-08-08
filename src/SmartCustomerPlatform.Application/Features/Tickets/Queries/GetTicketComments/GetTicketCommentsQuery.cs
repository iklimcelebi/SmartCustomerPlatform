using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketComments;

public record GetTicketCommentsQuery(
    Guid TicketId
) : IRequest<IReadOnlyList<Comment>>;

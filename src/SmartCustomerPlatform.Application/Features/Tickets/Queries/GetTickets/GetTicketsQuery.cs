using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTickets;

public record GetTicketsQuery()
    : IRequest<IReadOnlyList<Ticket>>;

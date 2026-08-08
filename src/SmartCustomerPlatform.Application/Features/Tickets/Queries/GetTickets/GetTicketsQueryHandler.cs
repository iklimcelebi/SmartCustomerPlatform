using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTickets;

public class GetTicketsQueryHandler
    : IRequestHandler<GetTicketsQuery, IReadOnlyList<Ticket>>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketsQueryHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<IReadOnlyList<Ticket>> Handle(
        GetTicketsQuery request,
        CancellationToken cancellationToken)
    {
        return await _ticketRepository.GetAllAsync();
    }
}

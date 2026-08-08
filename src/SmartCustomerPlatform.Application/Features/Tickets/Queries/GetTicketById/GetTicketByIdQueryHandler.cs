using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler
    : IRequestHandler<GetTicketByIdQuery, Ticket?>
{
    private readonly ITicketRepository _ticketRepository;

    public GetTicketByIdQueryHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Ticket?> Handle(
        GetTicketByIdQuery request,
        CancellationToken cancellationToken)
    {
        return await _ticketRepository.GetByIdAsync(request.Id);
    }
}

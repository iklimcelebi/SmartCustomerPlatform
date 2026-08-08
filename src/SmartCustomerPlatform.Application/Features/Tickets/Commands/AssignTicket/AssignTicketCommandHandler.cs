using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.AssignTicket;

public class AssignTicketCommandHandler
    : IRequestHandler<AssignTicketCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public AssignTicketCommandHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(
        AssignTicketCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            request.TicketId);

        if (ticket is null)
            throw new KeyNotFoundException(
                $"Ticket not found: {request.TicketId}");

        ticket.Assign(request.AssignedUserId);

        _ticketRepository.Update(ticket);

        await _ticketRepository.SaveChangesAsync();
    }
}

using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangeStatus;

public class ChangeStatusCommandHandler
    : IRequestHandler<ChangeStatusCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public ChangeStatusCommandHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(
        ChangeStatusCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            request.TicketId);

        if (ticket is null)
            throw new KeyNotFoundException(
                $"Ticket not found: {request.TicketId}");

        ticket.ChangeStatus(request.NewStatus);

        _ticketRepository.Update(ticket);

        await _ticketRepository.SaveChangesAsync();
    }
}
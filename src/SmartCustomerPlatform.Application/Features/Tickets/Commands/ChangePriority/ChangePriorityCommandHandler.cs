using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangePriority;

public class ChangePriorityCommandHandler
    : IRequestHandler<ChangePriorityCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public ChangePriorityCommandHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(
        ChangePriorityCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            request.TicketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException(
                $"Ticket not found: {request.TicketId}");
        }

        ticket.ChangePriority(request.NewPriority);

        _ticketRepository.Update(ticket);

        await _ticketRepository.SaveChangesAsync();
    }
}

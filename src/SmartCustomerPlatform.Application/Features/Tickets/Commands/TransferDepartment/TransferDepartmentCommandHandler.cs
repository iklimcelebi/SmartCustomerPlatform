using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.TransferDepartment;

public class TransferDepartmentCommandHandler
    : IRequestHandler<TransferDepartmentCommand>
{
    private readonly ITicketRepository _ticketRepository;

    public TransferDepartmentCommandHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task Handle(
        TransferDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(
            request.TicketId);

        if (ticket is null)
        {
            throw new KeyNotFoundException(
                $"Ticket not found: {request.TicketId}");
        }

        ticket.TransferDepartment(request.NewDepartmentId);

        _ticketRepository.Update(ticket);

        await _ticketRepository.SaveChangesAsync();
    }
}

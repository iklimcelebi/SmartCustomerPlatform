using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler
    : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository)
    {
        _ticketRepository = ticketRepository;
    }

    public async Task<Guid> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber =
                $"TCK-{DateTime.UtcNow:yyyyMMddHHmmssfff}",

            CustomerId = request.CustomerId,
            DepartmentId = request.DepartmentId,
            CategoryId = request.CategoryId,
            SubCategoryId = request.SubCategoryId,

            Subject = request.Subject,
            Description = request.Description
        };

        ticket.SetInitialPriority(request.Priority);

        ticket.InitializeSla(DateTime.UtcNow);

        ticket.AddDomainEvent(
            new TicketCreatedEvent(
                ticket.Id,
                ticket.TicketNumber,
                ticket.CustomerId,
                ticket.DepartmentId,
                ticket.CategoryId,
                ticket.SubCategoryId,
                ticket.Subject,
                ticket.Priority
            )
        );

        await _ticketRepository.AddAsync(ticket);

        return ticket.Id;
    }
}
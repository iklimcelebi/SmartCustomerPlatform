using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler
    : IRequestHandler<CreateTicketCommand, Guid>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ICustomerRepository _customerRepository;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        ICustomerRepository customerRepository)
    {
        _ticketRepository = ticketRepository;
        _customerRepository = customerRepository;
    }

    public async Task<Guid> Handle(
        CreateTicketCommand request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerRepository.GetByIdAsync(
                request.CustomerId);

        if (customer is null)
        {
            throw new InvalidOperationException(
                $"Customer not found: {request.CustomerId}");
        }

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

        var slaStartedAt = DateTime.UtcNow;

        ticket.InitializeSla(slaStartedAt);

        ticket.AddDomainEvent(
            new TicketCreatedEvent(
                ticket.Id,
                ticket.TicketNumber,
                ticket.CustomerId,
                $"{customer.FirstName} {customer.LastName}".Trim(),
                ticket.DepartmentId,
                ticket.CategoryId,
                ticket.SubCategoryId,
                ticket.Subject,
                ticket.Description,
                ticket.Priority,
                ticket.SlaStartedAt,
                ticket.SlaResponseDueAt,
                ticket.SlaResolutionDueAt
            )
        );

        await _ticketRepository.AddAsync(ticket);

        return ticket.Id;
    }
}

using MediatR;
using SmartCustomerPlatform.Application.Interfaces.Repositories;
using SmartCustomerPlatform.Domain.Entities;
using SmartCustomerPlatform.Domain.Services;

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
        var slaStartedAt = DateTime.UtcNow;

        var (responseTime, resolutionTime) =
            SlaPolicy.GetDurations(request.Priority);

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            TicketNumber = $"TCK-{DateTime.UtcNow:yyyyMMddHHmmssfff}",

            CustomerId = request.CustomerId,
            DepartmentId = request.DepartmentId,
            CategoryId = request.CategoryId,
            SubCategoryId = request.SubCategoryId,

            Subject = request.Subject,
            Description = request.Description,

            Priority = request.Priority,

            SlaStartedAt = slaStartedAt,
            SlaResponseDueAt = slaStartedAt.Add(responseTime),
            SlaResolutionDueAt = slaStartedAt.Add(resolutionTime),

            IsSlaPaused = false,
            TotalSlaPausedDuration = TimeSpan.Zero
        };

        await _ticketRepository.AddAsync(ticket);

        return ticket.Id;
    }
}

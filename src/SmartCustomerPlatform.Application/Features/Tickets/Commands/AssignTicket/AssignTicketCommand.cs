using MediatR;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.AssignTicket;

public record AssignTicketCommand(
    Guid TicketId,
    Guid AssignedUserId
) : IRequest;
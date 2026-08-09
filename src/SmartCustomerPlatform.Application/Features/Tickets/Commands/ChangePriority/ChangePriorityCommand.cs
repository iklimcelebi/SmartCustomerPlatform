using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangePriority;

public record ChangePriorityCommand(
    Guid TicketId,
    TicketPriority NewPriority
) : IRequest;

using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.ChangeStatus;

public record ChangeStatusCommand(
    Guid TicketId,
    TicketStatus NewStatus
) : IRequest;
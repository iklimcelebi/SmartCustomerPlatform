using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.CreateTicket;

public record CreateTicketCommand(
    Guid CustomerId,
    Guid DepartmentId,
    Guid CategoryId,
    Guid? SubCategoryId,
    string Subject,
    string Description,
    TicketPriority Priority
) : IRequest<Guid>;

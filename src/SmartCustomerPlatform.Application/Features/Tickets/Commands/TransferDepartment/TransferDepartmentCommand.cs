using MediatR;

namespace SmartCustomerPlatform.Application.Features.Tickets.Commands.TransferDepartment;

public record TransferDepartmentCommand(
    Guid TicketId,
    Guid NewDepartmentId
) : IRequest;

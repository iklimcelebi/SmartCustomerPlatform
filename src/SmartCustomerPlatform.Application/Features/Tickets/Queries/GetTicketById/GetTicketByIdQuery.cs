using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketById;

public record GetTicketByIdQuery(Guid Id)
    : IRequest<Ticket?>;

using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketEvents;

public record GetTicketEventsQuery(
    Guid TicketId
) : IRequest<IReadOnlyList<EventStoreEventDto>>;




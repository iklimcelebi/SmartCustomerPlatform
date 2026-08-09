using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;

namespace SmartCustomerPlatform.Application.Features.Tickets.Queries.GetTicketEvents;

public class GetTicketEventsQueryHandler
    : IRequestHandler<GetTicketEventsQuery, IReadOnlyList<EventStoreEventDto>>
{
    private readonly IEventStoreService _eventStoreService;

    public GetTicketEventsQueryHandler(
        IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task<IReadOnlyList<EventStoreEventDto>> Handle(
        GetTicketEventsQuery request,
        CancellationToken cancellationToken)
    {
        var streamName = $"ticket-{request.TicketId}";

        return await _eventStoreService.GetEventsAsync(
            streamName,
            cancellationToken);
    }
}


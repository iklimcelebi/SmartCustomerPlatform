using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketStatusChangedEventHandler
    : INotificationHandler<TicketStatusChangedEvent>
{
    private readonly IEventStoreService _eventStoreService;
    private readonly IElasticsearchService _elasticsearchService;

    public TicketStatusChangedEventHandler(
        IEventStoreService eventStoreService,
        IElasticsearchService elasticsearchService)
    {
        _eventStoreService = eventStoreService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task Handle(
        TicketStatusChangedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket status changed: {notification.TicketId}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketStatusChangedEvent),
            notification,
            cancellationToken);

        await _elasticsearchService.UpdateAsync(
            "tickets-v1",
            notification.TicketId.ToString(),
            new
            {
                status = notification.NewStatus.ToString()
            },
            cancellationToken);
    }
}
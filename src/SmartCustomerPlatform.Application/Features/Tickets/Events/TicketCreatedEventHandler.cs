using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketCreatedEventHandler
    : INotificationHandler<TicketCreatedEvent>
{
    private readonly IEventStoreService _eventStoreService;
    private readonly IElasticsearchService _elasticsearchService;

    public TicketCreatedEventHandler(
        IEventStoreService eventStoreService,
        IElasticsearchService elasticsearchService)
    {
        _eventStoreService = eventStoreService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task Handle(
        TicketCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket created: {notification.TicketNumber}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketCreatedEvent),
            notification,
            cancellationToken);

        await _elasticsearchService.IndexAsync(
            "tickets",
            notification.TicketId.ToString(),
            notification,
            cancellationToken);
    }
}
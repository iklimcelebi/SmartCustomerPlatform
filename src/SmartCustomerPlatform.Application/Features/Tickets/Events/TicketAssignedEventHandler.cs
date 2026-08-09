using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketAssignedEventHandler
    : INotificationHandler<TicketAssignedEvent>
{
    private readonly IEventStoreService _eventStoreService;
    private readonly IElasticsearchService _elasticsearchService;

    public TicketAssignedEventHandler(
        IEventStoreService eventStoreService,
        IElasticsearchService elasticsearchService)
    {
        _eventStoreService = eventStoreService;
        _elasticsearchService = elasticsearchService;
    }

    public async Task Handle(
        TicketAssignedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket assigned: {notification.TicketId}");

        // 1. EventStoreDB'ye event yaz
        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketAssignedEvent),
            notification,
            cancellationToken);

        // 2. Elasticsearch'teki ticket'ı güncelle
        await _elasticsearchService.UpdateAsync(
            "tickets",
            notification.TicketId.ToString(),
            new
            {
                assignedUserId = notification.AssignedUserId
            },
            cancellationToken);

        Console.WriteLine(
            $"[ELASTICSEARCH] Ticket assigned user updated: {notification.AssignedUserId}");
    }
}
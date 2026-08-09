using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketReopenedEventHandler
    : INotificationHandler<TicketReopenedEvent>
{
    private readonly IEventStoreService _eventStoreService;

    public TicketReopenedEventHandler(
        IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task Handle(
        TicketReopenedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket reopened: {notification.TicketId}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketReopenedEvent),
            notification,
            cancellationToken);
    }
}

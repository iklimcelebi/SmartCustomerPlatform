using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketClosedEventHandler
    : INotificationHandler<TicketClosedEvent>
{
    private readonly IEventStoreService _eventStoreService;

    public TicketClosedEventHandler(
        IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task Handle(
        TicketClosedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket closed: {notification.TicketId}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketClosedEvent),
            notification,
            cancellationToken);
    }
}
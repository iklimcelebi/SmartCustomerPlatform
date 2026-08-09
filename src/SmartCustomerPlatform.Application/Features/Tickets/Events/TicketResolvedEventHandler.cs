using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketResolvedEventHandler
    : INotificationHandler<TicketResolvedEvent>
{
    private readonly IEventStoreService _eventStoreService;

    public TicketResolvedEventHandler(
        IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task Handle(
        TicketResolvedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket resolved: {notification.TicketId}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketResolvedEvent),
            notification,
            cancellationToken);
    }
}
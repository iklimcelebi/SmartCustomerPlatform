using MediatR;
using SmartCustomerPlatform.Application.Interfaces.ExternalServices;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketAssignedEventHandler
    : INotificationHandler<TicketAssignedEvent>
{
    private readonly IEventStoreService _eventStoreService;

    public TicketAssignedEventHandler(
        IEventStoreService eventStoreService)
    {
        _eventStoreService = eventStoreService;
    }

    public async Task Handle(
        TicketAssignedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket assigned: {notification.TicketId} -> {notification.AssignedUserId}");

        await _eventStoreService.AppendEventAsync(
            $"ticket-{notification.TicketId}",
            nameof(TicketAssignedEvent),
            notification,
            cancellationToken);
    }
}
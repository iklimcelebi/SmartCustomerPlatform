using MediatR;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Application.Features.Tickets.Events;

public class TicketCreatedEventHandler
    : INotificationHandler<TicketCreatedEvent>
{
    public Task Handle(
        TicketCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(
            $"[DOMAIN EVENT] Ticket created: {notification.TicketNumber}");

        return Task.CompletedTask;
    }
}
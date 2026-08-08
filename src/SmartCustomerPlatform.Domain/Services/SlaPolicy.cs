using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Services;

public static class SlaPolicy
{
    public static (TimeSpan ResponseTime, TimeSpan ResolutionTime) GetDurations(
        TicketPriority priority)
    {
        return priority switch
        {
            TicketPriority.Low =>
                (TimeSpan.FromHours(24), TimeSpan.FromHours(72)),

            TicketPriority.Medium =>
                (TimeSpan.FromHours(8), TimeSpan.FromHours(48)),

            TicketPriority.High =>
                (TimeSpan.FromHours(4), TimeSpan.FromHours(24)),

            TicketPriority.Critical =>
                (TimeSpan.FromHours(1), TimeSpan.FromHours(8)),

            _ => throw new ArgumentOutOfRangeException(nameof(priority))
        };
    }
}
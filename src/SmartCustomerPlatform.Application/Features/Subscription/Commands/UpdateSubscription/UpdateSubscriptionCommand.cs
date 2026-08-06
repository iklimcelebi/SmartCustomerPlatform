using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.UpdateSubscription;

public record UpdateSubscriptionCommand(
    Guid Id,
    decimal MonthlyPrice,
    DateTime StartDate,
    DateTime? EndDate,
    Guid? CampaignId,
    decimal? DiscountedPrice,
    bool IsActive
) : IRequest<bool>;
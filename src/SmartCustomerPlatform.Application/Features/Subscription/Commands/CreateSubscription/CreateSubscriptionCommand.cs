using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    Guid CustomerId,
    Guid PackageId,
    decimal MonthlyPrice,
    DateTime StartDate,
    Guid? CampaignId,
    decimal? DiscountedPrice
) : IRequest<Guid>;
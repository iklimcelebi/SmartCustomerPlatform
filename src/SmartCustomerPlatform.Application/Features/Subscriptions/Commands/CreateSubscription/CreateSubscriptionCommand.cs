using MediatR;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Commands.CreateSubscription;

public record CreateSubscriptionCommand(
    string SubscriptionNumber,
    string PackageName,
    decimal MonthlyFee,
    decimal DiscountedPrice,
    int UsedQuota,
    int RemainingQuota,
    DateTime StartDate,
    DateTime? EndDate,
    bool IsAutoRenew,
    int TotalQuota,
    SubscriptionStatus Status,
    Guid CustomerId,
    Guid PackageId,
    Guid? CampaignId
) : IRequest<Guid>;


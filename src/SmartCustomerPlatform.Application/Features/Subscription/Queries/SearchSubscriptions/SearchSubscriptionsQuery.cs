using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

public record SearchSubscriptionsQuery(
    Guid? SubscriptionId,
    string? SubscriptionNumber,
    Guid? PackageId,
    string? Status,
    DateTime? StartDateFrom,
    DateTime? StartDateTo)
    : IRequest<List<SubscriptionSearchResultDto>>;
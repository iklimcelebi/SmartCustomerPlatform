using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

public record SearchSubscriptionsQuery(
    Guid? SubscriptionId,
    Guid? PackageId,
    string? Status,
    DateTime? StartDateFrom,
    DateTime? StartDateTo)
    : IRequest<List<SubscriptionSearchResultDto>>;
using MediatR;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetAllSubscriptions;

public record GetAllSubscriptionsQuery()
    : IRequest<List<SubscriptionDto>>;
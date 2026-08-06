using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionById;

public record GetSubscriptionByIdQuery(Guid Id)
    : IRequest<SubscriptionDto?>;
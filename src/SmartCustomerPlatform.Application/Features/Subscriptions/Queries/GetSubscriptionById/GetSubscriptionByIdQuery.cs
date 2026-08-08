using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptionById;

public record GetSubscriptionByIdQuery(Guid Id) : IRequest<Subscription?>;

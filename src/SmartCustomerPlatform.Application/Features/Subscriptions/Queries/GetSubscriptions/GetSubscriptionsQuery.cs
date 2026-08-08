using MediatR;
using SmartCustomerPlatform.Domain.Entities;

namespace SmartCustomerPlatform.Application.Features.Subscriptions.Queries.GetSubscriptions;

public record GetSubscriptionsQuery : IRequest<IReadOnlyList<Subscription>>;

using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;

public record GetSubscriptionDashboardQuery
    : IRequest<SubscriptionDashboardDto>;
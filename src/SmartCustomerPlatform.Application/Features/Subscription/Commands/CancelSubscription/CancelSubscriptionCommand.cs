using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.CancelSubscription;

public record CancelSubscriptionCommand(
    Guid Id) : IRequest<bool>;
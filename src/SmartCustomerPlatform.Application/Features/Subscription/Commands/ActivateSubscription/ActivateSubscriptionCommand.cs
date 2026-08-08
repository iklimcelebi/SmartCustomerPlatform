using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.ActivateSubscription;

public record ActivateSubscriptionCommand(
    Guid Id) : IRequest<bool>;
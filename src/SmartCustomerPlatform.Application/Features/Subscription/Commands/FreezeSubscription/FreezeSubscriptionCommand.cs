using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.FreezeSubscription;

public record FreezeSubscriptionCommand(
    Guid Id) : IRequest<bool>;
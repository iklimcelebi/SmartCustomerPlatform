using MediatR;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.ResumeSubscription;

public record ResumeSubscriptionCommand(
    Guid Id) : IRequest<bool>;
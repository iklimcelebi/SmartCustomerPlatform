using MediatR;
using SmartCustomerPlatform.Application.Common.Interfaces;

namespace SmartCustomerPlatform.Application.Features.Subscription.Commands.ChangePackage;

public class ChangePackageCommandHandler
    : IRequestHandler<ChangePackageCommand, bool>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public ChangePackageCommandHandler(
        ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<bool> Handle(
        ChangePackageCommand request,
        CancellationToken cancellationToken)
    {
        var subscription = await _subscriptionRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (subscription is null)
            return false;

        subscription.ChangePackage(
            request.PackageId,
            request.MonthlyPrice);

        await _subscriptionRepository.UpdateAsync(
            subscription,
            cancellationToken);

        return true;
    }
}
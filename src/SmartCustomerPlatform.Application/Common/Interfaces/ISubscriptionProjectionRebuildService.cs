namespace SmartCustomerPlatform.Application.Common.Interfaces;

public interface ISubscriptionProjectionRebuildService
{
    Task RebuildAsync(
        CancellationToken cancellationToken = default);
}
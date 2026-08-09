using SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;
using SmartCustomerPlatform.Application.Features.Subscription.Queries.SearchSubscriptions;

namespace SmartCustomerPlatform.Application.Common.Interfaces;

public interface ISubscriptionSearchService
{
    Task<List<SubscriptionSearchResultDto>> SearchAsync(
        Guid? subscriptionId,
        Guid? packageId,
        string? status,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        CancellationToken cancellationToken = default);

    Task<SubscriptionDashboardDto> GetDashboardAsync(
        CancellationToken cancellationToken = default);
}
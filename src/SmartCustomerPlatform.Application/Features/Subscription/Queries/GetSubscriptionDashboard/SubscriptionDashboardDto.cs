namespace SmartCustomerPlatform.Application.Features.Subscription.Queries.GetSubscriptionDashboard;

public class SubscriptionDashboardDto
{
    public int ActiveSubscriptionCount { get; set; }

    public int CancelledLast30DaysCount { get; set; }

    public List<ActiveSubscriptionByPackageDto> ActiveSubscriptionsByPackage { get; set; }
        = new();
}

public class ActiveSubscriptionByPackageDto
{
    public Guid PackageId { get; set; }

    public int Count { get; set; }
}
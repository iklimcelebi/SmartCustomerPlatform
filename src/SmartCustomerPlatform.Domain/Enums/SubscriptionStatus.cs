namespace SmartCustomerPlatform.Domain.Enums;

public enum SubscriptionStatus
{
    Draft = 1,
    PendingActivation = 2,
    Active = 3,
    Frozen = 4,
    PendingCancellation = 5,
    Cancelled = 6,
    Expired = 7
}
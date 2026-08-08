namespace SmartCustomerPlatform.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
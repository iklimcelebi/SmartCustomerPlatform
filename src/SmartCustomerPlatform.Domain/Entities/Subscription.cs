namespace SmartCustomerPlatform.Domain.Entities;

public class Subscription
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Guid CustomerId { get; private set; }

    public Guid PackageId { get; private set; }

    public decimal MonthlyPrice { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public bool IsActive { get; private set; }

    private Subscription()
    {
    }

    public Subscription(
        string name,
        Guid customerId,
        Guid packageId,
        decimal monthlyPrice,
        DateTime startDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        CustomerId = customerId;
        PackageId = packageId;
        MonthlyPrice = monthlyPrice;
        StartDate = startDate;
        IsActive = true;
    }
public void Activate()
{
    IsActive = true;
    EndDate = null;
}    
}
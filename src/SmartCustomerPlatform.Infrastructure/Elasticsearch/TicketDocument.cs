namespace SmartCustomerPlatform.Infrastructure.Elasticsearch;

public class TicketDocument
{
    public Guid TicketId { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? SubCategoryId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public int Priority { get; set; }

    public DateTime OccurredOn { get; set; }
}
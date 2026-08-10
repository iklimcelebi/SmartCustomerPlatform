namespace SmartCustomerPlatform.Application.Interfaces.ExternalServices;

public class TicketSearchResult
{
    public Guid TicketId { get; set; }

    public string TicketNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid DepartmentId { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? SubCategoryId { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime OccurredOn { get; set; }

    public Guid? AssignedUserId { get; set; }

    public DateTime SlaStartedAt { get; set; }

    public DateTime SlaResponseDueAt { get; set; }

    public DateTime SlaResolutionDueAt { get; set; }

    public bool IsSlaPaused { get; set; }

    public DateTime? SlaPausedAt { get; set; }

    public TimeSpan TotalSlaPausedDuration { get; set; }

    public bool IsSlaBreached { get; set; }
}

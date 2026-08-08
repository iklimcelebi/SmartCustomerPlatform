namespace SmartCustomerPlatform.Persistence.Projections;

public class ProjectionCheckpoint
{
    public Guid Id { get; set; }

    public string ProjectionName { get; set; } = string.Empty;

    public ulong CommitPosition { get; set; }

    public ulong PreparePosition { get; set; }

    public DateTime UpdatedAtUtc { get; set; }

    public Guid? LastEventId { get; set; }

    public string Status { get; set; } = "Running";

    public string? ErrorMessage { get; set; }
}
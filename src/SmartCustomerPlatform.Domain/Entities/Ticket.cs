using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Domain.Events;
using SmartCustomerPlatform.Domain.Services;

namespace SmartCustomerPlatform.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public Guid? AssignedUserId { get; private set; }

    public Guid CategoryId { get; set; }
    public TicketCategory Category { get; set; } = null!;

    public Guid? SubCategoryId { get; set; }
    public TicketSubCategory? SubCategory { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; private set; } = TicketStatus.Open;

    public TicketPriority Priority { get; private set; } =
        TicketPriority.Medium;

    // -------------------------
    // SLA
    // -------------------------

    public DateTime SlaStartedAt { get; private set; }

    public DateTime SlaResponseDueAt { get; private set; }

    public DateTime SlaResolutionDueAt { get; private set; }

    public bool IsSlaPaused { get; private set; }

    public DateTime? SlaPausedAt { get; private set; }

    public TimeSpan TotalSlaPausedDuration { get; private set; }

    public ICollection<Comment> Comments { get; set; } =
        new List<Comment>();

    // -------------------------
    // SLA Initialization
    // -------------------------

    public void InitializeSla(DateTime startedAt)
    {
        if (startedAt == default)
        {
            throw new ArgumentException(
                "SLA start date cannot be empty.",
                nameof(startedAt));
        }

        var durations =
            SlaPolicy.GetDurations(Priority);

        SlaStartedAt = startedAt;

        SlaResponseDueAt =
            startedAt.Add(durations.ResponseTime);

        SlaResolutionDueAt =
            startedAt.Add(durations.ResolutionTime);

        IsSlaPaused = false;
        SlaPausedAt = null;
        TotalSlaPausedDuration = TimeSpan.Zero;
    }

    // -------------------------
    // SLA Pause
    // -------------------------

    private void PauseSla(DateTime pausedAt)
    {
        if (IsSlaPaused)
            return;

        IsSlaPaused = true;
        SlaPausedAt = pausedAt;
    }

    // -------------------------
    // SLA Resume
    // -------------------------

    private void ResumeSla(DateTime resumedAt)
    {
        if (!IsSlaPaused || SlaPausedAt is null)
            return;

        var pausedDuration =
            resumedAt - SlaPausedAt.Value;

        if (pausedDuration < TimeSpan.Zero)
        {
            pausedDuration = TimeSpan.Zero;
        }

        TotalSlaPausedDuration += pausedDuration;

        SlaResponseDueAt =
            SlaResponseDueAt.Add(pausedDuration);

        SlaResolutionDueAt =
            SlaResolutionDueAt.Add(pausedDuration);

        IsSlaPaused = false;
        SlaPausedAt = null;
    }

    // -------------------------
    // Assignment
    // -------------------------

    public void Assign(Guid assignedUserId)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException(
                "Closed ticket cannot be assigned.");
        }

        if (assignedUserId == Guid.Empty)
        {
            throw new ArgumentException(
                "Assigned user ID cannot be empty.",
                nameof(assignedUserId));
        }

        if (AssignedUserId == assignedUserId)
            return;

        AssignedUserId = assignedUserId;

        AddDomainEvent(
            new TicketAssignedEvent(
                Id,
                assignedUserId));
    }

    // -------------------------
    // Department Transfer
    // -------------------------

    public void TransferDepartment(Guid newDepartmentId)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException(
                "Closed ticket cannot be transferred.");
        }

        if (newDepartmentId == Guid.Empty)
        {
            throw new ArgumentException(
                "Department ID cannot be empty.",
                nameof(newDepartmentId));
        }

        if (DepartmentId == newDepartmentId)
            return;

        var oldDepartmentId = DepartmentId;

        DepartmentId = newDepartmentId;

        AssignedUserId = null;

        AddDomainEvent(
            new TicketTransferredEvent(
                Id,
                oldDepartmentId,
                newDepartmentId));
    }

    // -------------------------
    // Priority
    // -------------------------

    public void ChangePriority(
        TicketPriority newPriority)
    {
        if (Status == TicketStatus.Closed)
        {
            throw new InvalidOperationException(
                "Closed ticket priority cannot be changed.");
        }

        if (Priority == newPriority)
            return;

        var oldPriority = Priority;

        Priority = newPriority;

        if (SlaStartedAt != default)
        {
            var durations =
                SlaPolicy.GetDurations(newPriority);

            var effectiveStart =
                SlaStartedAt.Add(
                    TotalSlaPausedDuration);

            SlaResponseDueAt =
                effectiveStart.Add(
                    durations.ResponseTime);

            SlaResolutionDueAt =
                effectiveStart.Add(
                    durations.ResolutionTime);

            if (IsSlaPaused &&
                SlaPausedAt is not null)
            {
                var currentPauseDuration =
                    DateTime.UtcNow -
                    SlaPausedAt.Value;

                if (currentPauseDuration >
                    TimeSpan.Zero)
                {
                    SlaResponseDueAt =
                        SlaResponseDueAt.Add(
                            currentPauseDuration);

                    SlaResolutionDueAt =
                        SlaResolutionDueAt.Add(
                            currentPauseDuration);
                }
            }
        }

        AddDomainEvent(
            new TicketPriorityChangedEvent(
                Id,
                oldPriority,
                newPriority));
    }

    // -------------------------
    // Initial Priority
    // -------------------------

    public void SetInitialPriority(
        TicketPriority priority)
    {
        Priority = priority;
    }

    // -------------------------
    // Status
    // -------------------------

    public void ChangeStatus(
        TicketStatus newStatus)
    {
        if (Status == newStatus)
            return;

        if (!IsValidStatusTransition(
                Status,
                newStatus))
        {
            throw new InvalidOperationException(
                $"Invalid ticket status transition: " +
                $"{Status} -> {newStatus}");
        }

        var oldStatus = Status;

        var now = DateTime.UtcNow;

        if (newStatus ==
            TicketStatus.WaitingForCustomer)
        {
            PauseSla(now);
        }

        if (Status ==
                TicketStatus.WaitingForCustomer &&
            newStatus ==
                TicketStatus.InProgress)
        {
            ResumeSla(now);
        }

        Status = newStatus;

        AddDomainEvent(
            new TicketStatusChangedEvent(
                Id,
                oldStatus,
                newStatus));

        if (newStatus ==
            TicketStatus.Resolved)
        {
            AddDomainEvent(
                new TicketResolvedEvent(Id));
        }

        if (newStatus ==
            TicketStatus.Closed)
        {
            AddDomainEvent(
                new TicketClosedEvent(Id));
        }

        if (oldStatus ==
            TicketStatus.Closed)
        {
            AddDomainEvent(
                new TicketReopenedEvent(Id));
        }
    }

    // -------------------------
    // Status Transition Rules
    // -------------------------

    private static bool IsValidStatusTransition(
        TicketStatus currentStatus,
        TicketStatus newStatus)
    {
        return currentStatus switch
        {
            TicketStatus.Open =>
                newStatus ==
                    TicketStatus.InProgress,

            TicketStatus.InProgress =>
                newStatus ==
                    TicketStatus.WaitingForCustomer ||
                newStatus ==
                    TicketStatus.Resolved,

            TicketStatus.WaitingForCustomer =>
                newStatus ==
                    TicketStatus.InProgress,

            TicketStatus.Resolved =>
                newStatus ==
                    TicketStatus.Closed ||
                newStatus ==
                    TicketStatus.InProgress,

            TicketStatus.Closed =>
                newStatus ==
                    TicketStatus.InProgress,

            _ => false
        };
    }
}
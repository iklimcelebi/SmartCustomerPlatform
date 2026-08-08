using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;
using SmartCustomerPlatform.Domain.Events;

namespace SmartCustomerPlatform.Domain.Entities;

public class Ticket : BaseEntity
{
    public string TicketNumber { get; set; } = string.Empty;

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    // Yeni: Talebin atandığı personelin ID'si
    public Guid? AssignedUserId { get; private set; }

    public Guid CategoryId { get; set; }
    public TicketCategory Category { get; set; } = null!;

    public Guid? SubCategoryId { get; set; }
    public TicketSubCategory? SubCategory { get; set; }

    public string Subject { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    public DateTime SlaStartedAt { get; set; }

    public DateTime SlaResponseDueAt { get; set; }

    public DateTime SlaResolutionDueAt { get; set; }

    public bool IsSlaPaused { get; set; }

    public DateTime? SlaPausedAt { get; set; }

    public TimeSpan TotalSlaPausedDuration { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    // Yeni: Talebi bir personele atar ve domain event üretir.
    public void Assign(Guid assignedUserId)
    {
        AssignedUserId = assignedUserId;

        AddDomainEvent(
            new TicketAssignedEvent(
                Id,
                assignedUserId
            ));
    }
}
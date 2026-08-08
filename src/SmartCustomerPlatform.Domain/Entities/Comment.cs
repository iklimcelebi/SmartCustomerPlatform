using SmartCustomerPlatform.Domain.Common;
using SmartCustomerPlatform.Domain.Enums;

namespace SmartCustomerPlatform.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid TicketId { get; set; }

    public Ticket Ticket { get; set; } = null!;

    public string Content { get; set; } = string.Empty;

    public string Author { get; set; } = string.Empty;

    public CommentType Type { get; set; }

    public bool IsDeleted { get; set; } = false;
}

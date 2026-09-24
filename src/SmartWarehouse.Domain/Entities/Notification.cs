using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// In-app notification for a specific user.
/// Created by business events (low stock, order approved, transfer completed, etc.).
/// Notifications can be marked as read individually or in bulk.
/// </summary>
public class Notification : BaseEntity<long>
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

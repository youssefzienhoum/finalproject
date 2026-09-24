using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Immutable audit trail record for every significant business operation.
/// Stores who did what, to which entity, and when.
/// Details field stores additional context as JSON for flexibility.
/// </summary>
public class AuditLog : BaseEntity<long>
{
    public string UserId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? Details { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}

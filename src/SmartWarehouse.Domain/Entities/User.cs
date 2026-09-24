using Microsoft.AspNetCore.Identity;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Application user extending ASP.NET Core Identity.
/// IdentityUser already provides Id (string GUID), UserName, Email, PasswordHash, etc.
/// We add domain-specific properties on top.
/// </summary>
public class User : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<WarehouseEmployee> WarehouseAssignments { get; set; } = new List<WarehouseEmployee>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}

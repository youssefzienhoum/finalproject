using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Stores refresh tokens for JWT token rotation.
/// Each user can have multiple active refresh tokens (e.g., from different devices).
/// Tokens are revoked on logout or when replaced via rotation.
/// </summary>
public class RefreshToken : BaseEntity<Guid>
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? RevokedReason { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Navigation
    public User User { get; set; } = null!;
}

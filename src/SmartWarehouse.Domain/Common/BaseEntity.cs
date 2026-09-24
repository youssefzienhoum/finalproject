namespace SmartWarehouse.Domain.Common;

/// <summary>
/// Base entity with a strongly-typed primary key.
/// All domain entities inherit from this to share Id, audit timestamps, and concurrency token.
/// </summary>
public abstract class BaseEntity<TKey> where TKey : struct
{
    public TKey Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// EF Core concurrency token — used for optimistic concurrency control.
    /// SQL Server auto-increments this on every UPDATE, and EF Core includes it in the WHERE clause.
    /// If another transaction modified the row, the WHERE won't match, triggering DbUpdateConcurrencyException.
    /// </summary>
   
}

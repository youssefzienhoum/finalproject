namespace SmartWarehouse.Domain.Enums;

/// <summary>
/// Stock transfer lifecycle states.
/// A transfer moves products between warehouses atomically.
/// </summary>
public enum TransferStatus
{
    Pending = 1,
    Completed = 2,
    Cancelled = 3
}

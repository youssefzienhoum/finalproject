namespace SmartWarehouse.Domain.Enums;

/// <summary>
/// Purchase order lifecycle states.
/// Valid transitions: Draft → Submitted → Approved → Received
///                    Draft → Cancelled, Submitted → Rejected/Cancelled, Approved → Cancelled
/// </summary>
public enum PurchaseOrderStatus
{
    Draft = 1,
    Submitted = 2,
    Approved = 3,
    Received = 4,
    Cancelled = 5,
    Rejected = 6
}

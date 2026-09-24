namespace SmartWarehouse.Domain.Enums;

/// <summary>
/// Sales order lifecycle states.
/// Valid transitions: Draft → Confirmed → Shipped → Completed
///                    Draft/Confirmed → Cancelled
/// </summary>
public enum SalesOrderStatus
{
    Draft = 1,
    Confirmed = 2,
    Shipped = 3,
    Completed = 4,
    Cancelled = 5
}

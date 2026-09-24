namespace SmartWarehouse.Domain.Enums;

/// <summary>
/// Types of physical stock movements recorded in the system.
/// Every quantity change in a warehouse generates exactly one InventoryMovement with one of these types.
/// </summary>
public enum MovementType
{
    Purchase = 1,
    Sale = 2,
    TransferIn = 3,
    TransferOut = 4,
    Adjustment = 5,
    Return = 6
}

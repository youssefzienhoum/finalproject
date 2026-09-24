using SmartWarehouse.Domain.Common;
using SmartWarehouse.Domain.Enums;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// A stock transfer moves products between two warehouses atomically.
/// Source and destination cannot be the same warehouse.
/// Completing a transfer creates paired TransferOut/TransferIn movements within a transaction.
/// </summary>
public class StockTransfer : BaseEntity<int>
{
    public string TransferNumber { get; set; } = string.Empty;
    public int SourceWarehouseId { get; set; }
    public int DestinationWarehouseId { get; set; }
    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public Warehouse SourceWarehouse { get; set; } = null!;
    public Warehouse DestinationWarehouse { get; set; } = null!;
    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}

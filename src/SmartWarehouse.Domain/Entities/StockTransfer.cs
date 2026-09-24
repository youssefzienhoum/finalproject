using SmartWarehouse.Domain.Common;
using SmartWarehouse.Domain.Entities;
using SmartWarehouse.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

public class StockTransfer : BaseEntity<int>
{
    public string TransferNumber { get; set; } = string.Empty;

    [ForeignKey(nameof(SourceWarehouse))]
    public int SourceWarehouseId { get; set; }

    [ForeignKey(nameof(DestinationWarehouse))]
    public int DestinationWarehouseId { get; set; }

    public TransferStatus Status { get; set; } = TransferStatus.Pending;
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? CompletedAt { get; set; }

    public Warehouse SourceWarehouse { get; set; } = null!;
    public Warehouse DestinationWarehouse { get; set; } = null!;
    public ICollection<StockTransferItem> Items { get; set; } = new List<StockTransferItem>();
}
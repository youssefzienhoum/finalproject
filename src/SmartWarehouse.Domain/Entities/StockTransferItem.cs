using SmartWarehouse.Domain.Common;

namespace SmartWarehouse.Domain.Entities;

/// <summary>
/// Line item in a stock transfer specifying product and quantity to move.
/// </summary>
public class StockTransferItem : BaseEntity<int>
{
    public int StockTransferId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation
    public StockTransfer StockTransfer { get; set; } = null!;
    public Product Product { get; set; } = null!;
}

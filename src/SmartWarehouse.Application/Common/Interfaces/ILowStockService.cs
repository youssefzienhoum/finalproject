namespace SmartWarehouse.Application.Common.Interfaces;

public interface ILowStockService
{
    Task CheckLowStockAndNotifyAsync(CancellationToken cancellationToken);
}

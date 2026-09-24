namespace SmartWarehouse.Application.Common.Interfaces;

public interface IDailyInventorySummaryJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

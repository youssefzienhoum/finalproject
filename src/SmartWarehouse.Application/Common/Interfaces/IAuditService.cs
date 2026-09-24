namespace SmartWarehouse.Application.Common.Interfaces;

public interface IAuditService
{
    Task LogAsync(string action, string entityName, string entityId, string details, string userId, CancellationToken cancellationToken = default);
}

namespace SmartWarehouse.Domain.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found in the database.
/// The global exception middleware maps this to HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException() : base() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    public NotFoundException(string entityName, object key)
        : base($"Entity \"{entityName}\" ({key}) was not found.") { }
}

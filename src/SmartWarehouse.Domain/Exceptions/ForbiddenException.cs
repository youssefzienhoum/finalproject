namespace SmartWarehouse.Domain.Exceptions;

/// <summary>
/// Thrown when the user is authenticated but does not have the required permissions.
/// Maps to HTTP 403 Forbidden.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException() : base() { }
    public ForbiddenException(string message) : base(message) { }
    public ForbiddenException(string message, Exception innerException) : base(message, innerException) { }
}

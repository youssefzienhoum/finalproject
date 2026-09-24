namespace SmartWarehouse.Domain.Exceptions;

/// <summary>
/// Thrown when the incoming request is invalid (e.g., malformed data or missing required fields).
/// Maps to HTTP 400 Bad Request.
/// </summary>
public class BadRequestException : Exception
{
    public BadRequestException() : base() { }
    public BadRequestException(string message) : base(message) { }
    public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
}

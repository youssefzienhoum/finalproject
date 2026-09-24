namespace SmartWarehouse.Domain.Exceptions;

/// <summary>
/// Thrown when an optimistic concurrency conflict occurs (e.g., RowVersion mismatch).
/// Maps to HTTP 409 Conflict.
/// </summary>
public class ConcurrencyException : Exception
{
    public ConcurrencyException() : base() { }
    public ConcurrencyException(string message) : base(message) { }
    public ConcurrencyException(string message, Exception innerException) : base(message, innerException) { }
}

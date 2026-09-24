namespace SmartWarehouse.Domain.Exceptions;

/// <summary>
/// Thrown when a business rule is violated (e.g., trying to ship an order without stock).
/// Maps to HTTP 400 Bad Request or HTTP 422 Unprocessable Entity depending on preference.
/// We will map it to 422 to distinguish from syntax/validation errors.
/// </summary>
public class BusinessRuleException : Exception
{
    public BusinessRuleException() : base() { }
    public BusinessRuleException(string message) : base(message) { }
    public BusinessRuleException(string message, Exception innerException) : base(message, innerException) { }
}

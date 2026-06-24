namespace Orders.Domain.Exceptions
{
    public abstract class DomainException(string message) 
        : Exception(message)
    {
    }

    public class OrderNotFoundException(Guid id) 
        : DomainException($"Order '{id}' was not found.")
    {
    }

    public class ProductNotFoundException(Guid productId) 
        : DomainException($"Product '{productId}' was not found.")
    {
    }

    public class InsufficientStockException(Guid productId, int requested, int available) 
        : DomainException($"Insufficient stock for product '{productId}'. Requested: {requested}, Available: {available}.")
    {
    }

    public class InvalidOrderOperationException(string message) 
        : DomainException(message)
    {
    }
}

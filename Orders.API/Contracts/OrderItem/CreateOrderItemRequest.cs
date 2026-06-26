namespace Orders.API.Contracts.OrderItem
{
    public record CreateOrderItemRequest(Guid ProductId, int Quantity);
}

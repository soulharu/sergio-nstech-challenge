using Orders.API.Contracts.OrderItem;

namespace Orders.API.Contracts.Order
{
    public record CreateOrderRequest(Guid CustomerId, string Currency, IEnumerable<CreateOrderItemRequest> Items);
}

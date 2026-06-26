namespace Orders.Application.Commands.CreateOrderItem
{
    public record CreateOrderItemCommand(Guid ProductId, int Quantity);
}

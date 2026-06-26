namespace Orders.Application.DTOs
{
    public record OrderItemDto(
    Guid Id,
    Guid ProductId,
    decimal UnitPrice,
    int Quantity,
    decimal Subtotal);
}

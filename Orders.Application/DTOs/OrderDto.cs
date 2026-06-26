using Orders.Domain.Entities;
using Orders.Domain.Enums;

namespace Orders.Application.DTOs
{
    public record OrderDto(
    Guid Id,
    Guid CustomerId,
    string Status,
    string Currency,
    decimal Total,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<OrderItemDto> Items)
    {
    public static OrderDto MapToDto(Order order) => new(
        order.Id,
        order.CustomerId,
        order.Status.ToString(),
        order.Currency,
        order.Total,
        order.CreatedAt,
        order.UpdatedAt,
        order.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.UnitPrice, i.Quantity, i.Subtotal)));
    }    
}

using Mediator;
using Orders.Application.Commands.CreateOrderItem;
using Orders.Application.DTOs;

namespace Orders.Application.Commands.CreateOrder
{
    public record CreateOrderCommand(
    Guid CustomerId,
    string Currency,
    IEnumerable<CreateOrderItemCommand> Items) : IRequest<OrderDto>;
}
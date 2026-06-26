using Mediator;
using Orders.Application.DTOs;

namespace Orders.Application.Commands.CancelOrder
{
    public record CancelOrderCommand(Guid OrderId) : IRequest<OrderDto>;
}

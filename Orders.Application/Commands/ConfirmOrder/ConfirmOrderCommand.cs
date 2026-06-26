using Mediator;
using Orders.Application.DTOs;

namespace Orders.Application.Commands.ConfirmOrder
{
    public record ConfirmOrderCommand(Guid OrderId) : IRequest<OrderDto>;
}

using Mediator;
using Orders.Application.DTOs;

namespace Orders.Application.Queries.GetOrderById
{
    public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;
}

using Mediator;
using Orders.Application.Common;
using Orders.Application.DTOs;
using Orders.Domain.Enums;

namespace Orders.Application.Queries.ListOrders
{
    public record ListOrdersQuery(
     Guid? CustomerId,
     OrderStatus? Status,
     DateTime? From,
     DateTime? To,
     int Page = 1,
     int PageSize = 20) : IRequest<PagedResult<OrderDto>>;
}

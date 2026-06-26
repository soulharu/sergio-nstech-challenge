using Mediator;
using Orders.Application.Common;
using Orders.Application.DTOs;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.Application.Queries.ListOrders
{
    public class ListOrdersHandler : IRequestHandler<ListOrdersQuery, PagedResult<OrderDto>>
    {
        private readonly IOrderRepository _orderRepository;

        public ListOrdersHandler(IOrderRepository orderRepository) =>
            _orderRepository = orderRepository;

        public async ValueTask<PagedResult<OrderDto>> Handle(ListOrdersQuery request, CancellationToken cancellationToken)
        {
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Clamp(request.PageSize, 1, 100);

            var (items, totalCount) = await _orderRepository.FilteredListWithPaginationAsync(
                request.CustomerId,
                request.Status,
                request.From,
                request.To,
                page,
                pageSize,
                cancellationToken);

            return new PagedResult<OrderDto>
            {
                Items = items.Select(OrderDto.MapToDto),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}

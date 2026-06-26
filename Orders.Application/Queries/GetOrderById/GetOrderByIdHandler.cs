using Mediator;
using Orders.Application.DTOs;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.Application.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository) =>
        _orderRepository = orderRepository;

    public async ValueTask<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(request.OrderId);

        return OrderDto.MapToDto(order);
    }
}

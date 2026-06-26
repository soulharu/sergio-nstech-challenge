using Mediator;
using Orders.Application.DTOs;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.Application.Commands.ConfirmOrder
{
    public class ConfirmOrderHandler : IRequestHandler<ConfirmOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ConfirmOrderHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<OrderDto> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
                ?? throw new OrderNotFoundException(request.OrderId);

            // idempotent — already confirmed
            if (order.Status == OrderStatus.Confirmed)
                return OrderDto.MapToDto(order);

            // Reserve stock for each item
            var productIds = order.Items.Select(i => i.ProductId);
            var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToList();

            foreach (var item in order.Items)
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId)
                    ?? throw new ProductNotFoundException(item.ProductId);

                product.ReserveStock(item.Quantity);
                _productRepository.Update(product);
            }

            order.Confirm();
            _orderRepository.Update(order);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OrderDto.MapToDto(order);
        }
    }
}

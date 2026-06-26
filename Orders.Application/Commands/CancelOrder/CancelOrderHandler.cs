using Mediator;
using Orders.Application.DTOs;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.Application.Commands.CancelOrder
{
    public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelOrderHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<OrderDto> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId, cancellationToken)
                ?? throw new OrderNotFoundException(request.OrderId);

            if (order.Status == OrderStatus.Canceled)
                return OrderDto.MapToDto(order);

            var wasConfirmed = order.Status == OrderStatus.Confirmed;

            order.Cancel();

            if (wasConfirmed)
            {
                var productIds = order.Items.Select(i => i.ProductId);
                var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToList();

                foreach (var item in order.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.ProductId)
                        ?? throw new ProductNotFoundException(item.ProductId);

                    product.ReleaseStock((int)item.Quantity);
                    _productRepository.Update(product);
                }
            }

            _orderRepository.Update(order);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OrderDto.MapToDto(order);
        }
    }
}

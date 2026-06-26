using Mediator;
using Orders.Application.DTOs;
using Orders.Domain.Entities;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async ValueTask<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var itemRequests = request.Items.ToList();

            if (itemRequests.Count == 0)
                throw new InvalidOrderOperationException("An order must have at least one item.");

            var productIds = itemRequests.Select(i => i.ProductId).Distinct();
            var products = (await _productRepository.GetByIdsAsync(productIds, cancellationToken)).ToList();

            var orderItems = new List<(Guid ProductId, decimal UnitPrice, int Quantity)>();

            foreach (var itemRequest in itemRequests)
            {
                var product = products.FirstOrDefault(p => p.Id == itemRequest.ProductId)
                    ?? throw new ProductNotFoundException(itemRequest.ProductId);

                if (product.AvailableQuantity < itemRequest.Quantity)
                    throw new InsufficientStockException(product.Id, itemRequest.Quantity, product.AvailableQuantity);

                orderItems.Add((product.Id, product.UnitPrice, itemRequest.Quantity));
            }

            var order = Order.Create(request.CustomerId, request.Currency, orderItems);
            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return OrderDto.MapToDto(order);
        }
    }
}

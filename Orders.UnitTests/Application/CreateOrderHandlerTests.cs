using FluentAssertions;
using Moq;
using Orders.Application.Commands.CreateOrder;
using Orders.Application.Commands.CreateOrderItem;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.UnitTests.Application
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private CreateOrderHandler CreateHandler() =>
            new(_orderRepo.Object, _productRepo.Object, _uow.Object);

        [Fact]
        public async Task Handle_WithValidRequest_ShouldCreatePlacedOrder()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var product = new Product(productId, "Test Product", 99.90m, 50);

            _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), default))
                .ReturnsAsync(new[] { product });
            _orderRepo.Setup(r => r.AddAsync(It.IsAny<Order>(), default)).Returns(Task.CompletedTask);
            _uow.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);

            var command = new CreateOrderCommand(
                Guid.NewGuid(), "BRL",
                new[] { new CreateOrderItemCommand(productId, 2) });

            var handler = CreateHandler();

            //Act
            var result = await handler.Handle(command, default);

            //Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(OrderStatus.Placed.ToString());
            result.Total.Should().Be(199.80m);
            _orderRepo.Verify(r => r.AddAsync(It.IsAny<Order>(), default), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentProduct_ShouldThrowProductNotFoundException()
        {
            //Arrange
            _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), default))
                .ReturnsAsync(Enumerable.Empty<Product>());

            var command = new CreateOrderCommand(
                Guid.NewGuid(), "BRL",
                new[] { new CreateOrderItemCommand(Guid.NewGuid(), 1) });

            var handler = CreateHandler();

            //Act
            var act = async () => await handler.Handle(command, default);

            //Assert
            await act.Should().ThrowAsync<ProductNotFoundException>();
        }

        [Fact]
        public async Task Handle_WithInsufficientStock_ShouldThrowInsufficientStockException()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var product = new Product(productId, "Test", 50m, 1);

            _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), default))
                .ReturnsAsync(new[] { product });

            var command = new CreateOrderCommand(
                Guid.NewGuid(), "BRL",
                new[] { new CreateOrderItemCommand(productId, 10) });

            var handler = CreateHandler();
            
            //Act
            var act = async () => await handler.Handle(command, default);

            //Assert
            await act.Should().ThrowAsync<InsufficientStockException>();
        }
    }
}

using FluentAssertions;
using Moq;
using Orders.Application.Commands.CancelOrder;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.UnitTests.Application
{
    public class CancelOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private CancelOrderHandler CreateHandler() =>
            new(_orderRepo.Object, _productRepo.Object, _uow.Object);

        [Fact]
        public async Task Handle_WhenAlreadyCanceled_ShouldBeIdempotentAndNotCallCommit()
        {
            //Arrange
            var order = Order.Create(Guid.NewGuid(), "BRL", new[] { (Guid.NewGuid(), 10m, 1) });
            order.Cancel();

            _orderRepo.Setup(r => r.GetByIdWithItemsAsync(order.Id, default)).ReturnsAsync(order);

            //Act
            var result = await CreateHandler().Handle(new CancelOrderCommand(order.Id), default);

            //Assert
            result.Status.Should().Be(OrderStatus.Canceled.ToString());
            _uow.Verify(u => u.CommitAsync(default), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenConfirmedOrder_ShouldReleaseStock()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var product = new Product(productId, "Test", 50m, 5);
            var order = Order.Create(Guid.NewGuid(), "BRL", new[] { (productId, 50m, 3) });
            order.Confirm();

            _orderRepo.Setup(r => r.GetByIdWithItemsAsync(order.Id, default)).ReturnsAsync(order);
            _productRepo.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>(), default))
                .ReturnsAsync(new[] { product });
            _uow.Setup(u => u.CommitAsync(default)).ReturnsAsync(1);

            //Act
            await CreateHandler().Handle(new CancelOrderCommand(order.Id), default);

            //Assert
            _productRepo.Verify(r => r.Update(It.IsAny<Product>()), Times.Once);
        }
    }
}

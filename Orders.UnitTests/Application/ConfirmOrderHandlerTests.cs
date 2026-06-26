using FluentAssertions;
using Moq;
using Orders.Application.Commands.ConfirmOrder;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;
using Orders.Domain.Interfaces.Repositories;

namespace Orders.UnitTests.Application
{
    public class ConfirmOrderHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepo = new();
        private readonly Mock<IProductRepository> _productRepo = new();
        private readonly Mock<IUnitOfWork> _uow = new();

        private ConfirmOrderHandler CreateHandler() =>
            new(_orderRepo.Object, _productRepo.Object, _uow.Object);

        [Fact]
        public async Task Handle_WhenOrderNotFound_ShouldThrowOrderNotFoundException()
        {
            //Arrange
            _orderRepo.Setup(r => r.GetByIdWithItemsAsync(It.IsAny<Guid>(), default))
                .ReturnsAsync((Order?)null);

            //Act
            var act = async () => await CreateHandler().Handle(new ConfirmOrderCommand(Guid.NewGuid()), default);
            
            //Assert
            await act.Should().ThrowAsync<OrderNotFoundException>();
        }

        [Fact]
        public async Task Handle_WhenAlreadyConfirmed_ShouldBeIdempotentAndNotCallCommit()
        {
            //Arrange
            var productId = Guid.NewGuid();
            var order = Order.Create(Guid.NewGuid(), "BRL", new[] { (productId, 10m, 1) });
            order.Confirm();

            _orderRepo.Setup(r => r.GetByIdWithItemsAsync(order.Id, default)).ReturnsAsync(order);

            //Act
            var result = await CreateHandler().Handle(new ConfirmOrderCommand(order.Id), default);

            //Assert
            result.Status.Should().Be(OrderStatus.Confirmed.ToString());
            _uow.Verify(u => u.CommitAsync(default), Times.Never);
        }
    }
}

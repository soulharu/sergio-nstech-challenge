using FluentAssertions;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Exceptions;

namespace Orders.UnitTests.Domain
{
    public class OrderTests
    {
        private static readonly Guid CustomerId = Guid.NewGuid();
        private static readonly Guid ProductId = Guid.NewGuid();

        private static List<(Guid ProductId, decimal UnitPrice, int Quantity)> DefaultItems() => new() { (ProductId, 100m, 2) };

        [Fact]
        public void Create_WithValidData_ShouldReturnPlacedOrder()
        {
            //Arrange & Act
            var order = Order.Create(CustomerId, "BRL", DefaultItems());

            //Assert
            order.Should().NotBeNull();
            order.CustomerId.Should().Be(CustomerId);
            order.Status.Should().Be(OrderStatus.Placed);
            order.Currency.Should().Be("BRL");
            order.Total.Should().Be(200m);
            order.Items.Should().HaveCount(1);
            order.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Create_WithNoItems_ShouldThrowDomainException()
        {
            //Arrange & Act
            var act = () => Order.Create(CustomerId, "BRL", Enumerable.Empty<(Guid, decimal, int)>());

            //Assert
            act.Should().Throw<InvalidOrderOperationException>()
                .WithMessage("*at least one item*");
        }

        [Fact]
        public void Create_CurrencyShouldBeUpperCase()
        {
            //Arrange & Act
            var order = Order.Create(CustomerId, "brl", DefaultItems());

            //Assert
            order.Currency.Should().Be("BRL");
        }

        [Fact]
        public void Confirm_WhenPlaced_ShouldTransitionToConfirmed()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());

            //Act
            order.Confirm();

            //Assert
            order.Status.Should().Be(OrderStatus.Confirmed);
            order.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Confirm_WhenAlreadyConfirmed_ShouldBeIdempotent()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());
            order.Confirm();
            var updatedAt = order.UpdatedAt;

            //Act
            order.Confirm();

            //Assert
            order.Status.Should().Be(OrderStatus.Confirmed);
            order.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void Confirm_WhenCanceled_ShouldThrowDomainException()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());
            order.Cancel();

            //Act
            var act = () => order.Confirm();

            //Assert
            act.Should().Throw<InvalidOrderOperationException>()
                .WithMessage("*Canceled*");
        }

        [Fact]
        public void Cancel_WhenPlaced_ShouldTransitionToCanceled()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());

            //Act
            order.Cancel();

            //Assert
            order.Status.Should().Be(OrderStatus.Canceled);
            order.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Cancel_WhenConfirmed_ShouldTransitionToCanceled()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());
            order.Confirm();

            //Act
            order.Cancel();

            //Assert
            order.Status.Should().Be(OrderStatus.Canceled);
        }

        [Fact]
        public void Cancel_WhenAlreadyCanceled_ShouldBeIdempotent()
        {
            //Arrange
            var order = Order.Create(CustomerId, "BRL", DefaultItems());
            order.Cancel();
            var updatedAt = order.UpdatedAt;

            //Act
            var act = () => order.Cancel();

            //Assert
            act.Should().NotThrow();
            order.Status.Should().Be(OrderStatus.Canceled);
            order.UpdatedAt.Should().Be(updatedAt);
        }

        [Fact]
        public void Total_ShouldBeCalculatedCorrectly()
        {
            //Arrange
            var items = new List<(Guid, decimal, int)>
            {
                (Guid.NewGuid(), 50m, 3), 
                (Guid.NewGuid(), 100m, 2),
            };

            //Act
            var order = Order.Create(CustomerId, "USD", items);

            //Assert
            order.Total.Should().Be(350m);
        }
    }
}

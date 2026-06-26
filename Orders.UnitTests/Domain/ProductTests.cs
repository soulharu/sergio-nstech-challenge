using FluentAssertions;
using Orders.Domain.Entities;
using Orders.Domain.Exceptions;

namespace Orders.UnitTests.Domain
{
    public class ProductTests
    {
        private static Product CreateProduct(int quantity = 10) => new(Guid.NewGuid(), "Test Product", 100m, quantity);

        [Fact]
        public void ReserveStock_WithSufficientQuantity_ShouldReduceAvailability()
        {
            //Arrange
            var product = CreateProduct(10);

            //Act
            product.ReserveStock(3);

            //Assert
            product.AvailableQuantity.Should().Be(7);
        }

        [Fact]
        public void ReserveStock_WithExactQuantity_ShouldSetAvailabilityToZero()
        {
            //Arrange
            var product = CreateProduct(5);

            //Act
            product.ReserveStock(5);

            //Assert
            product.AvailableQuantity.Should().Be(0);
        }

        [Fact]
        public void ReserveStock_WithInsufficientQuantity_ShouldThrow()
        {
            //Arrange
            var product = CreateProduct(2);

            //Act
            var act = () => product.ReserveStock(5);

            //Assert
            act.Should().Throw<InsufficientStockException>()
                .WithMessage("*Insufficient stock*");
        }

        [Fact]
        public void ReleaseStock_ShouldIncreaseAvailability()
        {
            //Arrange
            var product = CreateProduct(5);
            product.ReserveStock(3);

            //Act
            product.ReleaseStock(3);

            //Assert
            product.AvailableQuantity.Should().Be(5);
        }

        [Fact]
        public void Create_WithNegativePrice_ShouldThrow()
        {
            //Arrange & Act
            var act = () => new Product(Guid.NewGuid(), "Test", -1m, 10);

            //Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*price must be greater than zero*");
        }

        [Fact]
        public void Create_WithNegativeQuantity_ShouldThrow()
        {
            //Arrange & Act
            var act = () => new Product(Guid.NewGuid(), "Test", 100m, -1);

            //Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*quantity cannot be negative*");
        }
    }
}

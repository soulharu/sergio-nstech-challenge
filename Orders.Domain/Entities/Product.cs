using Orders.Domain.Exceptions;

namespace Orders.Domain.Entities
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int AvailableQuantity { get; set; }

        private Product() { Name = string.Empty; }

        public Product(Guid id, string name, decimal unitPrice, int availableQuantity)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Product name is required.");
            if (unitPrice <= 0) throw new ArgumentException("Unit price must be greater than zero.");
            if (availableQuantity < 0) throw new ArgumentException("Available quantity cannot be negative.");

            Id = id;
            Name = name;
            UnitPrice = unitPrice;
            AvailableQuantity = availableQuantity;
        }

        public void ReserveStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            if (AvailableQuantity < quantity)
                throw new InsufficientStockException(Id, quantity, AvailableQuantity);

            AvailableQuantity -= quantity;
        }

        public void ReleaseStock(int quantity)
        {
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");
            AvailableQuantity += quantity;
        }
    }
}

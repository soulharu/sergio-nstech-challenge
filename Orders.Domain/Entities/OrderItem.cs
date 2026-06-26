namespace Orders.Domain.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal => UnitPrice * Quantity;

        private OrderItem() { }

        public OrderItem(Guid orderId, Guid productId, decimal unitPrice, int quantity)
        {
            if (unitPrice <= 0) throw new ArgumentException("Unit price must be greater than zero.");
            if (quantity <= 0) throw new ArgumentException("Quantity must be greater than zero.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductId = productId;
            UnitPrice = unitPrice;
            Quantity = quantity;
        }
    }
}

using Orders.Domain.Enums;

namespace Orders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public string Currency { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        List<OrderItem> Items { get; set; }
    }
}

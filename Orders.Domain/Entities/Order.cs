using Orders.Domain.Enums;
using Orders.Domain.Exceptions;

namespace Orders.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public string Currency { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        private readonly List<OrderItem> _items = [];
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        private Order() { Currency = string.Empty; }

        public static Order Create(Guid customerId, string currency, IEnumerable<(Guid ProductId, decimal UnitPrice, int Quantity)> items)
        {
            if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.");

            var itemList = items.ToList();
            if (itemList.Count == 0)
                throw new InvalidOrderOperationException("An order must have at least one item.");

            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Currency = currency.ToUpperInvariant(),
                Status = OrderStatus.Placed,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var (productId, unitPrice, quantity) in itemList)
                order._items.Add(new OrderItem(order.Id, productId, unitPrice, quantity));

            order.RecalculateTotal();
            return order;
        }

        public void Confirm()
        {
            if (Status == OrderStatus.Confirmed) return;

            if (Status != OrderStatus.Placed)
                throw new InvalidOrderOperationException($"Cannot confirm an order in '{Status}' status. Only 'Placed' orders can be confirmed.");

            Status = OrderStatus.Confirmed;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Canceled) return;

            if (Status != OrderStatus.Placed && Status != OrderStatus.Confirmed)
                throw new InvalidOrderOperationException($"Cannot cancel an order in '{Status}' status.");

            Status = OrderStatus.Canceled;
            UpdatedAt = DateTime.UtcNow;
        }

        private void RecalculateTotal() => Total = _items.Sum(i => i.Subtotal);
    }
}

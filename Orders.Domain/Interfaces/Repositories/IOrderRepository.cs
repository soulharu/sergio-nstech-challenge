using Orders.Domain.Entities;
using Orders.Domain.Enums;

namespace Orders.Domain.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default);
        Task<(IEnumerable<Order> Items, int TotalCount)> FilteredListWithPaginationAsync(
            Guid? customerId, OrderStatus? status, DateTime? from, DateTime? to,
            int page, int pageSize, CancellationToken ct = default);
        Task AddAsync(Order order, CancellationToken ct = default);
        void Update(Order order);
    }
}

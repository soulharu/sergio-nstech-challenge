using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Enums;
using Orders.Domain.Interfaces.Repositories;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersDbContext _context;

        public OrderRepository(OrdersDbContext context) => _context = context;

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _context.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<Order?> GetByIdWithItemsAsync(Guid id, CancellationToken ct = default) =>
            await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id, ct);

        public async Task<(IEnumerable<Order> Items, int TotalCount)> FilteredListWithPaginationAsync(
            Guid? customerId, OrderStatus? status, DateTime? from, DateTime? to,
            int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Orders
                .Include(o => o.Items)
                .AsNoTracking()
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(o => o.CustomerId == customerId.Value);

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            if (from.HasValue)
                query = query.Where(o => o.CreatedAt >= from.Value.ToUniversalTime());

            if (to.HasValue)
                query = query.Where(o => o.CreatedAt <= to.Value.ToUniversalTime());

            var total = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task AddAsync(Order order, CancellationToken ct = default) =>
        await _context.Orders.AddAsync(order, ct);

        public void Update(Order order) =>
            _context.Orders.Update(order);
    }
}

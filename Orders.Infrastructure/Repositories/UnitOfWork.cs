using Orders.Domain.Interfaces.Repositories;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly OrdersDbContext _context;
        public UnitOfWork(OrdersDbContext context) => _context = context;
        public Task<int> CommitAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);
    }
}

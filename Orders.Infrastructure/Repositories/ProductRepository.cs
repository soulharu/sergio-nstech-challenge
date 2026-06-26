using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Domain.Interfaces.Repositories;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly OrdersDbContext _context;

        public ProductRepository(OrdersDbContext context) => _context = context;

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
            await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
        {
            var idList = ids.ToList();
            return await _context.Products
                .Where(p => idList.Contains(p.Id))
                .ToListAsync(ct);
        }

        public void Update(Product product) =>
            _context.Products.Update(product);
    }
}

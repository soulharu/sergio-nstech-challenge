using Orders.Domain.Entities;

namespace Orders.Domain.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
        void Update(Product product);
    }
}

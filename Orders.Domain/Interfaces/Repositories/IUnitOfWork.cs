namespace Orders.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync(CancellationToken ct = default);
    }
}

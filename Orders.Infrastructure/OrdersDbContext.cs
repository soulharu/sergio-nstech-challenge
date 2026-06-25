using Microsoft.EntityFrameworkCore;
using Orders.Domain.Entities;
using Orders.Infrastructure.Persistence.Configurations;

namespace Orders.Infrastructure;

public sealed class OrdersDbContext : DbContext
{
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

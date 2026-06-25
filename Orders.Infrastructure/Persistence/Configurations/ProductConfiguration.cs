using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            builder.Property(p => p.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
            builder.Property(p => p.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).IsRequired();
            builder.Property(p => p.AvailableQuantity).HasColumnName("available_quantity").IsRequired();
        }
    }
}

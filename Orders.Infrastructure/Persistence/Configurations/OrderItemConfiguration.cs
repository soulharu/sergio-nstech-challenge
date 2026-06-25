using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("order_items");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.ProductId).HasColumnName("product_id").IsRequired();
            builder.Property(i => i.UnitPrice).HasColumnName("unit_price").HasPrecision(18, 2).IsRequired();
            builder.Property(i => i.Quantity).HasColumnName("quantity").IsRequired();
            builder.Ignore(i => i.Subtotal);

            builder.HasIndex(i => i.Id).HasDatabaseName("ix_order_items_product_id");
            builder.HasOne<Product>()
            .WithMany()
            .HasForeignKey(i => i.ProductId);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");
            builder.Property(o => o.CustomerId).HasColumnName("customer_id").IsRequired();
            builder.Property(o => o.Status).HasColumnName("status").HasConversion<string>().IsRequired();
            builder.Property(o => o.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
            builder.Property(o => o.Total).HasColumnName("total").HasPrecision(18, 2).IsRequired();
            builder.Property(o => o.CreatedAt).HasColumnName("created_at").IsRequired();
            builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            builder.HasMany(o => o.Items)
                .WithOne()
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(o => o.CustomerId).HasDatabaseName("ix_orders_customer_id");
            builder.HasIndex(o => o.Status).HasDatabaseName("ix_orders_status");
            builder.HasIndex(o => o.CreatedAt).HasDatabaseName("ix_orders_created_at");
        }
    }
}

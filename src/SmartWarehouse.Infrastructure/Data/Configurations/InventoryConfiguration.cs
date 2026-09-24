using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        // Composite Primary Key
        builder.HasKey(i => new { i.ProductId, i.WarehouseId });

        // Optimistic concurrency token
        builder.Property(i => i.RowVersion)
            .IsRowVersion();

        // Index for performance
        builder.HasIndex(i => new { i.ProductId, i.WarehouseId });
    }
}

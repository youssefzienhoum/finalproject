using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.HasKey(im => im.Id);

        // Index on Product and Warehouse for fast querying of movement history
        builder.HasIndex(im => new { im.ProductId, im.WarehouseId });
        
       
    }
}

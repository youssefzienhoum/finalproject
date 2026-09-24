using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.HasKey(poi => poi.Id);

        builder.Property(poi => poi.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(poi => poi.TotalPrice).HasColumnType("decimal(18,2)");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);

        builder.Property(po => po.OrderNumber).IsRequired().HasMaxLength(50);
        
        builder.Property(po => po.TotalAmount).HasColumnType("decimal(18,2)");
        
        builder.HasIndex(po => po.Status);

        builder.Property(po => po.RowVersion).IsRowVersion();
    }
}

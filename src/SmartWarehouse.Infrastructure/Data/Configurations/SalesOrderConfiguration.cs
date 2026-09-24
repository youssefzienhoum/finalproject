using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(so => so.Id);

        builder.Property(so => so.OrderNumber).IsRequired().HasMaxLength(50);
        
        builder.Property(so => so.TotalAmount).HasColumnType("decimal(18,2)");

        builder.HasIndex(so => so.Status);

        
    }
}

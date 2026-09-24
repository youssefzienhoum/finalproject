using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class SalesOrderItemConfiguration : IEntityTypeConfiguration<SalesOrderItem>
{
    public void Configure(EntityTypeBuilder<SalesOrderItem> builder)
    {
        builder.HasKey(soi => soi.Id);

        builder.Property(soi => soi.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(soi => soi.TotalPrice).HasColumnType("decimal(18,2)");
    }
}

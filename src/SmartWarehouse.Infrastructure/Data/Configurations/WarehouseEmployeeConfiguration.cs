using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data.Configurations;

public class WarehouseEmployeeConfiguration : IEntityTypeConfiguration<WarehouseEmployee>
{
    public void Configure(EntityTypeBuilder<WarehouseEmployee> builder)
    {
        // Composite Primary Key ensures an employee can only be assigned to a warehouse once
        builder.HasKey(we => new { we.WarehouseId, we.UserId });

        builder.HasOne(we => we.Warehouse)
            .WithMany(w => w.Employees)
            .HasForeignKey(we => we.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(we => we.User)
            .WithMany(u => u.WarehouseAssignments)
            .HasForeignKey(we => we.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

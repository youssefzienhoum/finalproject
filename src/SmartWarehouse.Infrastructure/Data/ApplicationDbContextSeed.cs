using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartWarehouse.Domain.Entities;

namespace SmartWarehouse.Infrastructure.Data;

public static class ApplicationDbContextSeed
{
    public static async Task SeedDefaultUserAndRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

        // --- Seed Roles ---
        string[] roleNames = { "Admin", "WarehouseManager", "Employee", "Sales", "Purchasing" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new Role(roleName));
                logger.LogInformation("Role '{RoleName}' created.", roleName);
            }
        }

        // --- Seed Admin User (credentials from configuration) ---
        var adminEmail = configuration["SeedData:AdminEmail"] ?? "admin@smartwarehouse.com";
        var adminPassword = configuration["SeedData:AdminPassword"] ?? "Admin@123!";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            var newAdmin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Admin",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newAdmin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newAdmin, "Admin");
                logger.LogInformation("Admin user seeded successfully.");
            }
            else
            {
                logger.LogError("Failed to seed admin user. Errors: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // --- Seed Sample Demo Users ---
        var demoUsers = new (string Email, string FirstName, string LastName, string Role, string Password)[]
        {
            ("warehouse.manager@smartwarehouse.com", "Sarah", "Johnson", "WarehouseManager", "Manager@123!"),
            ("sales@smartwarehouse.com", "Mike", "Davis", "Sales", "Sales@123!"),
            ("purchasing@smartwarehouse.com", "Emily", "Chen", "Purchasing", "Purchasing@123!"),
            ("employee@smartwarehouse.com", "John", "Smith", "Employee", "Employee@123!")
        };

        foreach (var (email, firstName, lastName, role, password) in demoUsers)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    logger.LogInformation("Demo user '{Email}' with role '{Role}' seeded.", email, role);
                }
            }
        }

        // --- Seed Sample Business Data ---
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        await SeedSampleBusinessDataAsync(context, logger);
    }

    private static async Task SeedSampleBusinessDataAsync(ApplicationDbContext context, ILogger logger)
    {
        // --- Categories ---
        if (!await context.Categories.AnyAsync())
        {
            var categories = new[]
            {
                new Category { Name = "Electronics", Description = "Electronic components and devices" },
                new Category { Name = "Office Supplies", Description = "Paper, pens, and office essentials" },
                new Category { Name = "Raw Materials", Description = "Industrial raw materials" },
                new Category { Name = "Packaging", Description = "Boxes, tape, and packing materials" },
                new Category { Name = "Furniture", Description = "Office and warehouse furniture" }
            };
            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample categories seeded.");
        }

        // --- Suppliers ---
        if (!await context.Suppliers.AnyAsync())
        {
            var suppliers = new[]
            {
                new Supplier { Name = "TechParts Inc.", ContactName = "Alice Brown", Email = "alice@techparts.com", Phone = "+1-555-0101", Address = "123 Tech Ave, Silicon Valley, CA" },
                new Supplier { Name = "Global Office Supply", ContactName = "Bob Wilson", Email = "bob@globaloffice.com", Phone = "+1-555-0102", Address = "456 Business Rd, New York, NY" },
                new Supplier { Name = "Industrial Materials Co.", ContactName = "Carol White", Email = "carol@indmat.com", Phone = "+1-555-0103", Address = "789 Factory Ln, Detroit, MI" }
            };
            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample suppliers seeded.");
        }

        // --- Customers ---
        if (!await context.Customers.AnyAsync())
        {
            var customers = new[]
            {
                new Customer { Name = "Acme Corporation", ContactName = "Dan Miller", Email = "dan@acme.com", Phone = "+1-555-0201", Address = "100 Main St, Chicago, IL" },
                new Customer { Name = "Beta Industries", ContactName = "Eve Adams", Email = "eve@beta.com", Phone = "+1-555-0202", Address = "200 Oak Ave, Austin, TX" },
                new Customer { Name = "Gamma Solutions", ContactName = "Frank Lee", Email = "frank@gamma.com", Phone = "+1-555-0203", Address = "300 Pine St, Seattle, WA" }
            };
            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample customers seeded.");
        }

        // --- Warehouses ---
        if (!await context.Warehouses.AnyAsync())
        {
            var warehouses = new[]
            {
                new Warehouse { Name = "Main Warehouse", Location = "1000 Industrial Blvd, Dallas, TX", IsActive = true },
                new Warehouse { Name = "East Coast Hub", Location = "500 Harbor Dr, Newark, NJ", IsActive = true },
                new Warehouse { Name = "West Coast Hub", Location = "800 Pacific Hwy, Los Angeles, CA", IsActive = true }
            };
            context.Warehouses.AddRange(warehouses);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample warehouses seeded.");
        }

        // --- Products ---
        if (!await context.Products.AnyAsync())
        {
            var electronicsId = (await context.Categories.FirstAsync(c => c.Name == "Electronics")).Id;
            var officeId = (await context.Categories.FirstAsync(c => c.Name == "Office Supplies")).Id;
            var rawMatId = (await context.Categories.FirstAsync(c => c.Name == "Raw Materials")).Id;
            var packagingId = (await context.Categories.FirstAsync(c => c.Name == "Packaging")).Id;

            var products = new[]
            {
                new Product { SKU = "ELEC-001", Name = "Wireless Mouse", Description = "Ergonomic wireless mouse with USB receiver", CategoryId = electronicsId, Price = 29.99m, MinimumStockLevel = 50, IsActive = true },
                new Product { SKU = "ELEC-002", Name = "USB-C Hub", Description = "7-port USB-C hub with HDMI output", CategoryId = electronicsId, Price = 49.99m, MinimumStockLevel = 30, IsActive = true },
                new Product { SKU = "ELEC-003", Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard with Cherry MX switches", CategoryId = electronicsId, Price = 89.99m, MinimumStockLevel = 25, IsActive = true },
                new Product { SKU = "OFF-001", Name = "A4 Paper (Ream)", Description = "500 sheets white A4 paper, 80gsm", CategoryId = officeId, Price = 5.99m, MinimumStockLevel = 200, IsActive = true },
                new Product { SKU = "OFF-002", Name = "Ballpoint Pen (Box)", Description = "Box of 50 blue ballpoint pens", CategoryId = officeId, Price = 12.99m, MinimumStockLevel = 100, IsActive = true },
                new Product { SKU = "RAW-001", Name = "Steel Sheet (1m²)", Description = "1mm thickness cold-rolled steel sheet", CategoryId = rawMatId, Price = 35.00m, MinimumStockLevel = 100, IsActive = true },
                new Product { SKU = "PKG-001", Name = "Cardboard Box (Medium)", Description = "Medium corrugated cardboard box 30x25x20cm", CategoryId = packagingId, Price = 1.50m, MinimumStockLevel = 500, IsActive = true },
                new Product { SKU = "PKG-002", Name = "Bubble Wrap Roll", Description = "10m roll of bubble wrap", CategoryId = packagingId, Price = 8.99m, MinimumStockLevel = 50, IsActive = true }
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
            logger.LogInformation("Sample products seeded.");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using SmartWarehouse.Application;
using SmartWarehouse.Infrastructure;
using SmartWarehouse.API.Middleware;
using Hangfire;
using SmartWarehouse.Application.Common.Interfaces;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// Service Registration
// ---------------------------------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger / OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Smart Warehouse & Inventory Management API",
        Version = "v1",
        Description = @"A professional .NET 8 Clean Architecture Web API showcasing:
- **CQRS** with MediatR
- **EF Core** with SQL Server
- **JWT Authentication** & Refresh Tokens
- **Role-Based Authorization**
- **FluentValidation**
- **Hangfire** background jobs
- **Optimistic Concurrency** for inventory operations
- **Transactional** stock operations (receive, ship, transfer)
- **Specification Pattern**, Repository + Unit of Work",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Smart Warehouse Team"
        }
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.\n\nEnter: **Bearer {your_token}**",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments from API project for endpoint documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// Clean Architecture Layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

// ---------------------------------------------------------------------------
// Middleware Pipeline
// ---------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Smart Warehouse API v1");
        c.DocumentTitle = "Smart Warehouse API - Swagger";
    });
}

// Global Exception Handling (before everything else)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ---------------------------------------------------------------------------
// Database Migration & Seeding
// ---------------------------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<SmartWarehouse.Infrastructure.Data.ApplicationDbContext>();
        if (context.Database.IsSqlServer())
        {
            await context.Database.MigrateAsync();
        }
        await SmartWarehouse.Infrastructure.Data.ApplicationDbContextSeed.SeedDefaultUserAndRolesAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// ---------------------------------------------------------------------------
// Hangfire Recurring Jobs
// ---------------------------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

    // Job 1 — Low Stock Check (every hour)
    recurringJobManager.AddOrUpdate<ILowStockService>(
        "LowStockCheckJob",
        service => service.CheckLowStockAndNotifyAsync(CancellationToken.None),
        Cron.Hourly);

    // Job 2 — Daily Inventory Summary (every day at midnight UTC)
    recurringJobManager.AddOrUpdate<IDailyInventorySummaryJob>(
        "DailyInventorySummaryJob",
        job => job.ExecuteAsync(CancellationToken.None),
        Cron.Daily);
}

app.Run();

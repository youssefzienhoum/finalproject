using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace SmartWarehouse.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(assembly);
            // We can add behaviors here later (e.g., ValidationBehavior, LoggingBehavior)
        });

        return services;
    }
}

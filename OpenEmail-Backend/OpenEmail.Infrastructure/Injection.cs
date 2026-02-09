using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenEmail.Infrastructure.Persistence;

namespace OpenEmail.Infrastructure;

public static class Injection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        ArgumentNullException.ThrowIfNull(connectionString);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}
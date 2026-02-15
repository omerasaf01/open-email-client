using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Infrastructure.EmailProviders;
using OpenEmail.Infrastructure.Persistence;

namespace OpenEmail.Infrastructure;

public static class Injection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        ArgumentNullException.ThrowIfNull(connectionString);
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<IEmailProviderFactory, EmailProviderFactory>();
        
        return services;
    }
}
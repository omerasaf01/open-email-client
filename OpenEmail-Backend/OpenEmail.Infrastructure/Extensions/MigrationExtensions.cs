using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenEmail.Infrastructure.Persistence;

namespace OpenEmail.Infrastructure.Extensions;

public static class MigrationExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            await dbContext.Database.MigrateAsync();
            app.Logger.LogInformation("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            app.Logger.LogError(ex, "Migration failed: {Message}", ex.Message);
            throw;
        }
    }
}
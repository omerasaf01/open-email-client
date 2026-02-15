using Microsoft.EntityFrameworkCore;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Domain.Entities;

namespace OpenEmail.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<EmailAccount> EmailAccounts => Set<EmailAccount>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
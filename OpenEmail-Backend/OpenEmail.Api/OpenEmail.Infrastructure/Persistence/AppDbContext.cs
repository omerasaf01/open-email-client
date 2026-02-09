using Microsoft.EntityFrameworkCore;

namespace OpenEmail.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
}
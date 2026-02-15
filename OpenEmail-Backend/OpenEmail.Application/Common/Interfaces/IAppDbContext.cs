using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OpenEmail.Domain.Entities;

namespace OpenEmail.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<EmailAccount> EmailAccounts { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
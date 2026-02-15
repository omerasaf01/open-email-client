using OpenEmail.Domain.Entities;

namespace OpenEmail.Application.Common.Interfaces;

public interface IEmailProviderFactory
{
    IEmailProvider GetProviderAsync(EmailAccount emailAccount);
}
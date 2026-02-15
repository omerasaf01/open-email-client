using MailKit.Net.Imap;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Domain.Entities;
using OpenEmail.Domain.Enums;
using OpenEmail.Infrastructure.EmailProviders.Imap;

namespace OpenEmail.Infrastructure.EmailProviders;

public class EmailProviderFactory : IEmailProviderFactory
{
    public IEmailProvider GetProviderAsync(EmailAccount emailAccount)
    {
        if (emailAccount.ProviderType == EmailAccountProviders.Imap)
            return GetProvider(emailAccount);
        
        throw new NotSupportedException($"Email provider {emailAccount.ProviderType} is not supported");
    }
    
    private IEmailProvider GetProvider(EmailAccount emailAccount)
    {
        var client = new ImapClient();
        
        return new GenericImapProvider(client, emailAccount);
    }
}
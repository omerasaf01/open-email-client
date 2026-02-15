using OpenEmail.Domain.Enums;

namespace OpenEmail.Domain.Entities;

public class EmailAccount
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public EmailAccountProviders ProviderType { get; set; } = EmailAccountProviders.Imap;
    public string ImapHost { get; set; } = string.Empty;
    public int ImapPort { get; set; } = 993;
    public bool ImapUseSsl { get; set; } = false;
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; } = 587;
    public bool SmtpUseSsl { get; set; } = true;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
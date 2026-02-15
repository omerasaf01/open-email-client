using MailKit;
using MailKit.Net.Imap;
using MailKit.Security;
using MimeKit;
using OpenEmail.Application.Common.Dtos;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Domain.Entities;

namespace OpenEmail.Infrastructure.EmailProviders.Imap;

public class GenericImapProvider(ImapClient imapClient, EmailAccount emailAccount) : IEmailProvider
{
    public async Task ConnectAsync()
    {
        try
        {
            Console.WriteLine($"Connecting to {emailAccount.ImapHost}:{emailAccount.ImapPort} SSL:{emailAccount.ImapUseSsl}");

            imapClient.Timeout = 10000;
            imapClient.CheckCertificateRevocation = false;

            await imapClient.ConnectAsync(
                emailAccount.ImapHost,
                emailAccount.ImapPort,
                SecureSocketOptions.Auto);

            Console.WriteLine("Connected, authenticating...");

            await imapClient.AuthenticateAsync(
                           emailAccount.Email,
                           emailAccount.Password);

            Console.WriteLine("Authenticated successfully");
        }
        catch (Exception e)
        {
            throw;
        }
    }

    public async Task DisconnectAsync()
    {
        await imapClient.DisconnectAsync(true);
    }

    public async Task<List<EmailSummaryDto>> FetchInboxAsync(int count = 50, CancellationToken ct = default)
    {
        await ConnectAsync();
        var inbox = imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadOnly, ct);
        var messages = await inbox.FetchAsync(0, count, MessageSummaryItems.Full | MessageSummaryItems.UniqueId, ct);
        await DisconnectAsync();
        var result = messages.Select(e => new EmailSummaryDto
        {
            Id = e.UniqueId.ToString(),
            Subject = e.Envelope.Subject,
            From = e.Envelope.From.ToString(),
            ReceivedAt = e.Envelope.Date?.UtcDateTime ?? DateTime.MinValue,
            IsRead = e.Flags.HasValue && e.Flags.Value.HasFlag(MessageFlags.Seen),
            HasAttachments = e.Attachments != null && e.Attachments.Any(),
            FolderOrLabel = "Inbox"
        }).ToList();

        return result;
    }

    public async Task<EmailMessageDto> GetEmailByIdAsync(string uid, CancellationToken ct = default)
    {
        await ConnectAsync();
        var inbox = imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadOnly, ct);

        if (!UniqueId.TryParse(uid, out var uniqueId))
            throw new ArgumentException("Invalid UID");

        var message = await inbox.GetMessageAsync(uniqueId, ct);
        var emailMessageDto = new EmailMessageDto
        {
            Id = message.MessageId,
            Subject = message.Subject,
            From = message.From.ToString(),
            ReceivedAt = message.Date.UtcDateTime,
            HasAttachments = message.Attachments != null && message.Attachments.Any(),
            FolderOrLabel = "Inbox",
            To = message.To.Select(e => e.ToString()).ToArray(),
            BodyHtml = message.HtmlBody
        };

        if (message.Attachments == null)
            return emailMessageDto;

        foreach (var attachment in message.Attachments.OfType<MimePart>())
        {
            using var ms = new MemoryStream();
            await attachment.Content.DecodeToAsync(ms, ct);
            emailMessageDto.Attachments.Add(new EmailAttachmentDto
            {
                FileName = attachment.FileName,
                ContentType = attachment.ContentType.MimeType,
                Size = ms.Length,
                Data = ms.ToArray()
            });
        }

        return emailMessageDto;
    }

    public async Task MarkAsReadAsync(string uid, CancellationToken ct = default)
    {
        await ConnectAsync();
        var inbox = imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

        if (!UniqueId.TryParse(uid, out var uniqueId))
            throw new ArgumentException("Invalid UID");

        await inbox.AddFlagsAsync(uniqueId, MessageFlags.Seen, true, ct);
    }

    public async Task DeleteAsync(string uid, CancellationToken ct = default)
    {
        await ConnectAsync();
        var inbox = imapClient.Inbox;
        await inbox.OpenAsync(FolderAccess.ReadWrite, ct);

        if (!UniqueId.TryParse(uid, out var uniqueId))
            throw new ArgumentException("Invalid UID");

        await inbox.AddFlagsAsync(uniqueId, MessageFlags.Deleted, true, ct);
    }
}

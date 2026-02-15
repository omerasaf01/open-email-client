using OpenEmail.Application.Common.Dtos;

namespace OpenEmail.Application.Common.Interfaces;

public interface IEmailProvider
{
    Task ConnectAsync();
    Task DisconnectAsync();

    Task<List<EmailSummaryDto>> FetchInboxAsync(int count = 50, CancellationToken ct = default);
    //Task GetEmailByIdAsync(string uid, CancellationToken ct = default);
    Task MarkAsReadAsync(string uid, CancellationToken ct = default);
    Task DeleteAsync(string uid, CancellationToken ct = default);

    //Task SendAsync(EmailSendRequest request, CancellationToken ct = default);
}
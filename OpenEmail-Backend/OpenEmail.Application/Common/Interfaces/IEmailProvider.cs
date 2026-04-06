using OpenEmail.Application.Common.Dtos;

namespace OpenEmail.Application.Common.Interfaces;

public interface IEmailProvider
{
    /// <summary>
    /// Connect 
    /// </summary>
    /// <returns></returns>
    Task ConnectAsync();
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task DisconnectAsync();
    
    /// <summary>
    /// Fetch user's email inbox
    /// </summary>
    /// <param name="count"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<List<EmailSummaryDto>> FetchInboxAsync(int count = 100, CancellationToken ct = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task MarkAsReadAsync(string uid, CancellationToken ct = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task DeleteAsync(string uid, CancellationToken ct = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<EmailMessageDto> GetEmailByIdAsync(string uid, CancellationToken ct = default);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task SendAsync(string to, string subject, string body, CancellationToken ct = default);
}
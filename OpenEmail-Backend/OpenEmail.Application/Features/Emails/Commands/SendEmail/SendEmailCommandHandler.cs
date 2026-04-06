using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenEmail.Application.Common.Extensions;
using OpenEmail.Application.Common.Interfaces;

namespace OpenEmail.Application.Features.Emails.Commands.SendEmail;

public class SendEmailCommandHandler(
    IHttpContextAccessor http,
    ILogger<SendEmailCommandHandler> logger,
    IEmailProviderFactory emailProviderFactory,
    IAppDbContext dbContext)
    : ICommandHandler<SendEmailCommand>
{
    private readonly HttpContext _httpContext = http.HttpContext!;
    
    public async Task ExecuteAsync(SendEmailCommand command, CancellationToken ct)
    {
        var accountId = _httpContext.User.GetId();
        var account = await dbContext.EmailAccounts.FindAsync([accountId], ct);
        
        if (account == null)
            throw new UnauthorizedAccessException("Email account not found");
        
        var emailProvider =  emailProviderFactory.GetProviderAsync(account);
        await emailProvider.SendAsync(command.To, command.Subject, command.Body, ct);

        await Task.CompletedTask;
    }
}
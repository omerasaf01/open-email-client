using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenEmail.Application.Common.Extensions;
using OpenEmail.Application.Common.Interfaces;

namespace OpenEmail.Application.Features.Emails.Queries.GetEmailById;

public class GetEmailByIdQueryHandler(
    IEmailProviderFactory emailProviderFactory,
    IHttpContextAccessor http,
    ILogger<GetEmailByIdQueryHandler> logger,
    IAppDbContext dbContext)
    : ICommandHandler<GetEmailByIdQuery, GetEmailByIdResult>
{
    private readonly HttpContext _httpContext = http.HttpContext!; // It can not be null
    
    public async Task<GetEmailByIdResult> ExecuteAsync(GetEmailByIdQuery command, CancellationToken ct)
    {
        var emailId = _httpContext.User.GetId();
        var emailAccount = await dbContext.EmailAccounts.FindAsync([emailId], ct);
        
        if (emailAccount is null)
        {
            logger.LogWarning("Email account not found for user with id {EmailId}", emailId);
            throw new KeyNotFoundException("Email account not found");
        }
        
        var emailProvider = emailProviderFactory.GetProviderAsync(emailAccount);
        var email = await emailProvider.GetEmailByIdAsync(command.Id, ct);

        return new GetEmailByIdResult(email);
    }
}
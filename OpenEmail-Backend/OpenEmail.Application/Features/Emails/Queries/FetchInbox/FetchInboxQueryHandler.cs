using FastEndpoints;
using Microsoft.AspNetCore.Http;
using OpenEmail.Application.Common.Extensions;
using OpenEmail.Application.Common.Interfaces;
using IMapper = AutoMapper.IMapper;

namespace OpenEmail.Application.Features.Emails.Queries.FetchInbox;

public class FetchInboxQueryHandler(
    IMapper mapper,
    IAppDbContext dbContext,
    IEmailProviderFactory emailProviderFactory,
    IHttpContextAccessor http)
    : ICommandHandler<FetchInboxQuery, FetchInboxResult>
{
    private readonly HttpContext _httpContext = http.HttpContext!;
    
    public async Task<FetchInboxResult> ExecuteAsync(FetchInboxQuery command, CancellationToken ct)
    {
        var emailId = _httpContext.User.GetId();
        var emailAccount = await dbContext.EmailAccounts.FindAsync([emailId], ct);
        
        if (emailAccount is null)
            throw new KeyNotFoundException("Email account not found");
        
        var emailProvider = emailProviderFactory.GetProviderAsync(emailAccount);
        var emailSummaries = await emailProvider.FetchInboxAsync(ct: ct);

        return new FetchInboxResult(emailSummaries);;
    }
}
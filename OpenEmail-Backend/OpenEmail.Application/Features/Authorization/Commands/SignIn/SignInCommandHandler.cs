using FastEndpoints;
using FastEndpoints.Security;
using OpenEmail.Application.Common.Interfaces;
using OpenEmail.Domain.Entities;

namespace OpenEmail.Application.Features.Authorization.Commands.SignIn;

public class SignInCommandHandler(IAppDbContext dbContext, IEmailProviderFactory emailProviderFactory)
    : ICommandHandler<SignInCommand, SignInResult>
{
    public async Task<SignInResult> ExecuteAsync(SignInCommand command, CancellationToken ct)
    {
        var imapHost = Environment.GetEnvironmentVariable("IMAP_HOST");
        ArgumentException.ThrowIfNullOrWhiteSpace(imapHost);
        var emailAccount = new EmailAccount
        {
            Email = command.Email,
            ImapHost = imapHost,
            Password = command.Password,
        };
        var emailClient = emailProviderFactory.GetProviderAsync(emailAccount);
        await emailClient.ConnectAsync();
        var accessToken = CreateAccessToken(command.Email);
        emailAccount.AccessToken = accessToken;
        await dbContext.EmailAccounts.AddAsync(emailAccount, ct);
        await dbContext.SaveChangesAsync(ct);

        return new SignInResult(accessToken);
    }

    private static string CreateAccessToken(string email)
    {
        var signingKey = Environment.GetEnvironmentVariable("JWT_SECRET");
        ArgumentException.ThrowIfNullOrWhiteSpace(signingKey);
        var jwtToken = JwtBearer.CreateToken(opt =>
        {
            opt.SigningKey = signingKey;
            opt.ExpireAt = DateTime.UtcNow.AddDays(1);
            opt.User.Claims.Add(("Email", email));
        });

        return jwtToken;
    }
}
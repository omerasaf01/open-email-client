using FastEndpoints;
using OpenEmail.Api.Contracts.Requests;
using OpenEmail.Api.Contracts.Responses;
using OpenEmail.Application.Features.Authorization.Commands.SignIn;
using IMapper = AutoMapper.IMapper;

namespace OpenEmail.Api.Endpoints.Auth;

public class SignInEndpoint(IMapper mapper) : Endpoint<SignInRequest, SignInResponse>
{
    public override void Configure()
    {
        Post("/api/auth/signin");
        Summary(s => s.Summary = "Sign in to the application");
        AllowAnonymous();
    }

    public override async Task HandleAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var signInCommandResult = await new SignInCommand(request.Email, request.Password)
            .ExecuteAsync(cancellationToken);
        var signInResponse = mapper.Map<SignInResponse>(signInCommandResult);

        await Send.OkAsync(signInResponse, cancellation: cancellationToken);
    }
}
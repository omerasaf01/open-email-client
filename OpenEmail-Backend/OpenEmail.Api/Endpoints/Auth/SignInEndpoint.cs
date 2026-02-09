using FastEndpoints;
using MediatR;
using OpenEmail.Api.Contracts.Requests;
using OpenEmail.Api.Contracts.Responses;
using OpenEmail.Application.Features.Authorization.Commands.SignIn;
using IMapper = AutoMapper.IMapper;

namespace OpenEmail.Api.Endpoints.Auth;

public class SignInEndpoint(ISender sender, IMapper mapper) : Endpoint<SignInRequest, SignInResponse>
{
    public override void Configure() => Post("/api/auth/signin");

    public override async Task<SignInResponse> HandleAsync(SignInRequest request, CancellationToken cancellationToken)
    {
        var signInCommand = mapper.Map<SignInCommand>(request);
        var result = await sender.Send(signInCommand, cancellationToken);
        
        return mapper.Map<SignInResponse>(result);
    }
}
using MediatR;

namespace OpenEmail.Application.Features.Authorization.Commands.SignIn;

public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResult>
{
    public async Task<SignInResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        
    }
}
using MediatR;

namespace OpenEmail.Application.Features.Authorization.Commands.SignIn;

public record SignInCommand(string Email, string Password) : IRequest<SignInResult>;
using FastEndpoints;

namespace OpenEmail.Application.Features.Authorization.Commands.SignIn;

public record SignInCommand(string Email, string Password) : ICommand<SignInResult>;
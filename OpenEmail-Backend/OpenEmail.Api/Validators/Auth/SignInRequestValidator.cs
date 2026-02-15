using FastEndpoints;
using FluentValidation;
using OpenEmail.Api.Contracts.Requests;

namespace OpenEmail.Api.Validators.Auth;

public class SignInRequestValidator : Validator<SignInRequest>
{
    public SignInRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .NotNull().WithMessage("Email cannot be null")
            .EmailAddress();
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .NotNull().WithMessage("Password cannot be null");
    }
}
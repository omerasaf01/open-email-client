using AutoMapper;
using OpenEmail.Api.Contracts.Responses;
using OpenEmail.Application.Features.Authorization.Commands.SignIn;

namespace OpenEmail.Api.Mappings;

public class SignInProfile : Profile
{
    public SignInProfile()
    {
        CreateMap<SignInResult, SignInResponse>();
    }
}

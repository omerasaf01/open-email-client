namespace OpenEmail.Api.Contracts.Responses;

public record SignInResponse
{
    public required string AccessToken { get; set; }
}
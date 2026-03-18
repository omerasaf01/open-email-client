using System.Security.Claims;

namespace OpenEmail.Application.Common.Extensions;

public static class UserExtensions
{
    public static Guid GetId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(x => x.Type == "Id")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
            throw new UnauthorizedAccessException("User is not valid");
        
        return Guid.Parse(userId);
    }
}
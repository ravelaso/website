using System.Security.Claims;

namespace website.Services;

public class AuthorizationService
{
    private readonly string[] _allowedUsers =
        (Environment.GetEnvironmentVariable("GITHUB_USERS")
         ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries);

    public bool IsAuthorized(ClaimsPrincipal user)
    {
        if (!user.Identity?.IsAuthenticated ?? false)
            return false;

        var username = user.FindFirst("login")?.Value;
        return !string.IsNullOrEmpty(username) && _allowedUsers.Contains(username);
    }

    public string? GetAvatar(ClaimsPrincipal user)
    {
        return user.FindFirst("avatar_url")?.Value;
    }
    public string? GetUserName(ClaimsPrincipal user)
    {
        return user.FindFirst("name")?.Value ?? user.FindFirst("login")?.Value;
    }

    public string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst("id")?.Value;
    }

    public string? GetUserLogin(ClaimsPrincipal user)
    {
        return user.FindFirst("login")?.Value;
    }
}
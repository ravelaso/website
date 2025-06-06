using System.Security.Claims;
namespace website.Services
{
    public class AuthorizationService(DataService dataService)
    {
        public bool IsAuthorized(ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated ?? false)
                return false;

            var username = user.FindFirst("login")?.Value;
            var userId = user.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(userId))
                return false;

            // Check against the database instead of a hardcoded list
            return dataService.IsUserAllowed(userId, username);
        }

        public static string? GetAvatar(ClaimsPrincipal user)
        {
            return user.FindFirst("avatar_url")?.Value;
        }

        public static string? GetUserName(ClaimsPrincipal user)
        {
            return user.FindFirst("name")?.Value ?? user.FindFirst("login")?.Value;
        }

        public static string? GetUserId(ClaimsPrincipal user)
        {
            return user.FindFirst("id")?.Value;
        }

        public static string? GetUserLogin(ClaimsPrincipal user)
        {
            return user.FindFirst("login")?.Value;
        }
    }
}
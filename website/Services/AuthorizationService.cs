using System.Security.Claims;
namespace website.Services
{
    public class AllowedUser
    {
        public required string Username { get; init; }
        public required string Id { get; init; }
    }

    public class AuthorizationService
    {
        private readonly List<AllowedUser> _allowedUsers;

        public AuthorizationService(IConfiguration configuration)
        {
            // Fetching the AllowedUsers section from configuration and deserializing it
            _allowedUsers = configuration.GetSection("AllowedUsers")
                .Get<List<AllowedUser>>() ?? [];
        }

        public bool IsAuthorized(ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated ?? false)
                return false;

            var username = user.FindFirst("login")?.Value;
            var userId = user.FindFirst("id")?.Value;

            return !string.IsNullOrEmpty(username) &&
                   !string.IsNullOrEmpty(userId) &&
                   _allowedUsers.Any(u => u.Username == username && u.Id == userId);
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
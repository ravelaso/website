using System.Security.Claims;
namespace website.Services
{
    public class AuthorizationService(IConfiguration configuration)
    {
        private readonly List<(string Username, string Id)> _allowedUsers =
            configuration.GetSection("AllowedUsers")
            .Get<List<(string Username, string Id)>>() ?? [];

        public bool IsAuthorized(ClaimsPrincipal user)
        {
            if (!user.Identity?.IsAuthenticated ?? false)
                return false;

            var username = user.FindFirst("login")?.Value;
            var userId = user.FindFirst("id")?.Value;

            Console.WriteLine($"user: {username}, id: {userId}");

            foreach (var aUser in _allowedUsers)
            {
                Console.WriteLine($"Username: {aUser.Username}, ID: {aUser.Id}");
            }

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
using System.Security.Claims;
namespace website.Services
{
    public class AllowedUser
    {
        public string Username { get; set; }
        public string Id { get; set; }
    }

    public class AuthorizationService
    {
        private readonly List<AllowedUser> _allowedUsers;

        public AuthorizationService(IConfiguration configuration)
        {
            // Fetching the AllowedUsers section from configuration and deserializing it
            _allowedUsers = configuration.GetSection("AllowedUsers")
                .Get<List<AllowedUser>>() ?? [];
            #if DEBUG
            // Log the loaded allowed users for debugging
            Console.WriteLine("Loaded Allowed Users:");
            foreach (var user in _allowedUsers)
            {
                Console.WriteLine($"Username: {user.Username}, ID: {user.Id}");
            }
            #endif
        }

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
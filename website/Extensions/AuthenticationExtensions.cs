using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;

namespace website.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddGitHubAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = "GitHub";
            })
            .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true; // Prevents JavaScript access to the cookie
                    options.Cookie.SecurePolicy =
                        CookieSecurePolicy.Always; // Ensures the cookie is sent only over HTTPS
                    options.Cookie.SameSite = SameSiteMode.Strict; // Mitigates CSRF attacks
                    options.LoginPath = "/auth/login"; // Redirect to login page
                    options.LogoutPath = "/auth/logout"; // Redirect to logout page
                    options.ExpireTimeSpan = TimeSpan.FromHours(1); // Set cookie expiration
                    options.SlidingExpiration = true; // Reset expiration time on each request
                }
            )
            .AddOAuth("GitHub", options => { ConfigureGitHubOptions(options, configuration); });

        return services;
    }

    private static void ConfigureGitHubOptions(OAuthOptions options, IConfiguration configuration)
    {
        options.ClientId = configuration["GITHUB_CLIENT_ID"] ?? string.Empty;
        if (string.IsNullOrEmpty(options.ClientId))
        {
            Console.WriteLine("GITHUB_CLIENT_ID not configured");
        }

        options.ClientSecret = configuration["GITHUB_CLIENT_SECRET"] ?? string.Empty;
        if (string.IsNullOrEmpty(options.ClientSecret))
        {
            Console.WriteLine("GITHUB_CLIENT_SECRET not configured");
        }

        options.CallbackPath = new PathString(configuration["OAuth:CallbackPath"] ?? string.Empty);
        if (string.IsNullOrEmpty(options.CallbackPath.Value))
        {
            Console.WriteLine("OAuth:CallbackPath not configured");
        }

        options.AuthorizationEndpoint = configuration["OAuth:AuthorizationEndpoint"] ?? string.Empty;
        if (string.IsNullOrEmpty(options.AuthorizationEndpoint))
        {
            Console.WriteLine("OAuth:AuthorizationEndpoint not configured");
        }

        options.TokenEndpoint = configuration["OAuth:TokenEndpoint"] ?? string.Empty;
        if (string.IsNullOrEmpty(options.TokenEndpoint))
        {
            Console.WriteLine("OAuth:TokenEndpoint not configured");
        }

        options.UserInformationEndpoint = configuration["OAuth:UserInformationEndpoint"] ?? string.Empty;
        if (string.IsNullOrEmpty(options.UserInformationEndpoint))
        {
            Console.WriteLine("OAuth:UserInformationEndpoint not configured");
        }

        var scope = configuration["OAuth:Scope"] ?? string.Empty;
        if (!string.IsNullOrEmpty(scope))
        {
            options.Scope.Add(scope);
        }
        else
        {
            Console.WriteLine("OAuth:Scope not configured");
        }

        ConfigureClaimMappings(options);
        ConfigureOAuthEvents(options);
    }

    private static void ConfigureClaimMappings(OAuthOptions options)
    {
        options.ClaimActions.MapJsonKey("login", "login");
        options.ClaimActions.MapJsonKey("id", "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey("avatar_url", "avatar_url");
    }

    private static void ConfigureOAuthEvents(OAuthOptions options)
    {
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new("application/json"));
                request.Headers.Authorization = new("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead,
                    context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

#if DEBUG
                Console.WriteLine(response.Content.ReadAsStringAsync().Result);
#endif

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);
            }
        };
    }
}
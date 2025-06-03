
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System.Security.Claims;
using System.Text.Json;

namespace website.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddGitHubAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = "GitHub";
        })
        .AddCookie()
        .AddOAuth("GitHub", options =>
        {
            ConfigureGitHubOptions(options, configuration);
        });

        return services;
    }

    private static void ConfigureGitHubOptions(OAuthOptions options, IConfiguration configuration)
    {
        options.ClientId = configuration["GITHUB_CLIENT_ID"]
                           ?? string.Empty; Console.WriteLine("GITHUB_CLIENT_ID not configured");

        options.ClientSecret = configuration["GITHUB_CLIENT_SECRET"]
                               ?? string.Empty; Console.WriteLine("GITHUB_CLIENT_SECRET not configured");

        options.CallbackPath = new PathString(configuration["OAuth:CallbackPath"] ?? string.Empty);

        options.AuthorizationEndpoint = configuration["OAuth:AuthorizationEndpoint"]
                                        ?? string.Empty; Console.WriteLine("OAuth:AuthorizationEndpoint not configured");

        options.TokenEndpoint = configuration["OAuth:TokenEndpoint"]
                                ?? string.Empty; Console.WriteLine("OAuth:TokenEndpoint not configured");

        options.UserInformationEndpoint = configuration["OAuth:UserInformationEndpoint"]
                                          ?? string.Empty; Console.WriteLine("OAuth:UserInformationEndpoint not configured");


        options.Scope.Add(configuration["OAuth:Scope"] ?? string.Empty);

        ConfigureClaimMappings(options);
        ConfigureOAuthEvents(options);
    }

    private static void ConfigureClaimMappings(OAuthOptions options)
    {
        options.ClaimActions.MapJsonKey("login", "login");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        options.ClaimActions.MapJsonKey("avatar_url", "avatar_url");
    }

    private static void ConfigureOAuthEvents(OAuthOptions options)
    {
        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
                request.Headers.Accept.Add(new ("application/json"));
                request.Headers.Authorization = new ("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);
            }
        };
    }
}
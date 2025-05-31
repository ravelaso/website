
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
            ?? throw new InvalidOperationException("GITHUB_CLIENT_ID not configured");
        options.ClientSecret = configuration["GITHUB_CLIENT_SECRET"]
            ?? throw new InvalidOperationException("GITHUB_CLIENT_SECRET not configured");

        options.CallbackPath = new PathString("/auth/github/callback");

        options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
        options.TokenEndpoint = "https://github.com/login/oauth/access_token";
        options.UserInformationEndpoint = "https://api.github.com/user";

        options.Scope.Add("user:email");

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
                request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
                response.EnsureSuccessStatusCode();

                var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                context.RunClaimActions(user.RootElement);
            }
        };
    }
}
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace website.Extensions;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this WebApplication app)
    {
        app.MapGet("/auth/login", HandleLogin);
        app.MapGet("/auth/logout", HandleLogout);
    }

    private static async Task HandleLogin(HttpContext context)
    {
        await context.ChallengeAsync("GitHub", new AuthenticationProperties
        {
            RedirectUri = "/dashboard"
        });
    }

    private static async Task HandleLogout(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        context.Response.Redirect("/");
    }
}
using website.Services;

namespace website.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();

        services.AddScoped<AuthorizationService>();
        services.AddScoped<ImageService>();
        services.AddScoped<DataService>();
        services.AddScoped<NotificationService>();

        return services;
    }
}
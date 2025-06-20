using website.Blazor;
using website.Extensions;
using Serilog;
using website.Models;
using website.Services;

// Load environment variables from .env file
DotNetEnv.Env.Load();


// Set up Serilog logger
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
    .Enrich.FromLogContext()
    .MinimumLevel.Information()// Adjust as needed
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Use Serilog
builder.Host.UseSerilog();

// Add environment variables to configuration
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDbContext<AppDbContext>();

// Add services to the container
builder.Services.AddApplicationServices();
builder.Services.AddGitHubAuthentication(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();

    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Map endpoints
app.MapAuthenticationEndpoints();
app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
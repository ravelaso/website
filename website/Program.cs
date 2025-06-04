using website.Blazor;
using website.Extensions;
using Serilog;
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

// Add services to the container
builder.Services.AddApplicationServices();
builder.Services.AddGitHubAuthentication(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
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

ThumbnailGenerator.GenerateThumbnails(
    "wwwroot/images/gallery/",
    "wwwroot/images/thumbs/"
);

app.Run();
using HomeTrack.IdentityServer;
using HomeTrack.IdentityServer.Data;
using HomeTrack.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var connectionString = configuration["DbConnection"];
var serverVersion = new MySqlServerVersion(new Version(configuration["DatabaseSettings:ServerVersion"]
    ?? throw new NullReferenceException("server version was null")));

builder.Services.AddDbContext<AppDbContext>(options => 
{
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddIdentity<AppUser, IdentityRole>(config =>
{
    config.Password.RequiredLength = 4;
    config.Password.RequireDigit = false;
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddIdentityServer()
    .AddAspNetIdentity<AppUser>()
    .AddInMemoryApiResources(Configuration.ApiResources)
    .AddInMemoryIdentityResources(Configuration.IdentityResources)
    .AddInMemoryApiScopes(Configuration.ApiScopes)
    .AddInMemoryClients(Configuration.Clients)
    .AddDeveloperSigningCredential();

builder.Services.ConfigureApplicationCookie(config =>
{
    config.Cookie.Name = "HomeTrack.Identity.Cookie";
    config.LoginPath = "/auth/login";
    config.LogoutPath = "/auth/logout";
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
try
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);
}
catch (Exception ex)
{
    var logger = scope.ServiceProvider?.GetRequiredService<ILogger<Program>>();
    logger?.LogError(ex, "An exception occured while app initialization");
}

app.MapGet("/", () => "Hello World!");

app.UseIdentityServer();
app.UseRouting();

app.Run();
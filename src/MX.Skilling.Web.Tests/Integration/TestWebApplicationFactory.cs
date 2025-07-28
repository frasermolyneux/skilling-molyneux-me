using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MX.Skilling.Web.Tests.Integration;

/// <summary>
/// Custom web application factory for testing without authentication.
/// </summary>
/// <typeparam name="TStartup">The startup class type.</typeparam>
public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    /// <summary>
    /// Configures the web host for testing.
    /// </summary>
    /// <param name="builder">The web host builder.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "AzureAd:Instance", "https://login.microsoftonline.com/" },
            { "AzureAd:TenantId", "test-tenant-id" },
            { "AzureAd:ClientId", "test-client-id" },
            { "AzureAd:ClientSecret", "test-client-secret" },
            { "AzureAd:CallbackPath", "/signin-oidc" },
            { "AdminUserPrincipalNames:0", "admin@not-this-user.com" }, // Non-admin configuration for default tests
            { "KeyVaultUri", "" } // Disable Key Vault integration during testing
        }));

        builder.ConfigureServices(services => services.AddAuthentication("Test")
            .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthenticationSchemeHandler>(
                "Test", _ => { }));

        builder.UseEnvironment("Testing");
    }
}

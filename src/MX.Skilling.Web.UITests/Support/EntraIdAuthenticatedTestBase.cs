using Microsoft.Playwright;
using MX.Skilling.Web.UITests.Services;
using Xunit;

namespace MX.Skilling.Web.UITests.Support;

/// <summary>
/// Base class for UI tests using real Entra ID authentication.
/// </summary>
[Trait("Category", "UI")]
public abstract class EntraIdAuthenticatedTestBase : PlaywrightTestBase
{
    /// <summary>
    /// Gets the Entra ID test authenticator instance.
    /// </summary>
    protected EntraIdTestAuthenticator? Authenticator { get; private set; }

    /// <summary>
    /// Initializes the test with Entra ID authentication.
    /// </summary>
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Initialize the authenticator with Key Vault
        var keyVaultUri = Environment.GetEnvironmentVariable("KEY_VAULT_URI")
                         ?? "https://kv-skilling-dev-e3r7w4.vault.azure.net/";

        Authenticator = new EntraIdTestAuthenticator(keyVaultUri);
        await Authenticator.InitializeAsync();

        // Enable UI testing mode in the web application
        Environment.SetEnvironmentVariable("UITesting__Enabled", "true");
        Environment.SetEnvironmentVariable("UITesting__TenantId", await GetTenantIdAsync());
    }

    /// <summary>
    /// Authenticates the browser context as an admin user.
    /// </summary>
    protected async Task AuthenticateAsAdminAsync()
    {
        if (Authenticator == null)
        {
            throw new InvalidOperationException("Authenticator not initialized");
        }

        var token = await Authenticator.GetAccessTokenAsync(UserType.Admin);

        await Context!.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        });
    }

    /// <summary>
    /// Authenticates the browser context as a regular user.
    /// </summary>
    protected async Task AuthenticateAsUserAsync()
    {
        if (Authenticator == null)
        {
            throw new InvalidOperationException("Authenticator not initialized");
        }

        var token = await Authenticator.GetAccessTokenAsync(UserType.User);

        await Context!.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        });
    }

    /// <summary>
    /// Performs interactive authentication (for debugging/manual testing).
    /// </summary>
    protected async Task AuthenticateInteractivelyAsync(UserType userType)
    {
        if (Authenticator == null)
        {
            throw new InvalidOperationException("Authenticator not initialized");
        }

        var token = await Authenticator.GetInteractiveAccessTokenAsync(userType);

        await Context!.SetExtraHTTPHeadersAsync(new Dictionary<string, string>
        {
            ["Authorization"] = $"Bearer {token}"
        });
    }

    private Task<string> GetTenantIdAsync()
    {
        // This would typically come from Key Vault as well
        var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID")
               ?? "e56a6947-bb9a-4a6e-846a-1f118d1c3a14"; // Your tenant ID
        return Task.FromResult(tenantId);
    }
}

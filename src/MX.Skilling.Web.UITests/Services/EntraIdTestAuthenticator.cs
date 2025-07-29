using System.Text.Json;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Identity.Client;

namespace MX.Skilling.Web.UITests.Services;

/// <summary>
/// Handles real Entra ID authentication for UI tests using app registrations from Key Vault.
/// </summary>
public class EntraIdTestAuthenticator
{
    private readonly SecretClient _keyVaultClient;
    private string? _tenantId;
    private readonly string _cacheDirectory;

    /// <summary>
    /// Initializes a new instance of the EntraIdTestAuthenticator class.
    /// </summary>
    /// <param name="keyVaultUri">The URI of the Key Vault containing test credentials.</param>
    public EntraIdTestAuthenticator(string keyVaultUri)
    {
        var credential = new DefaultAzureCredential();
        _keyVaultClient = new SecretClient(new Uri(keyVaultUri), credential);
        _cacheDirectory = Path.Combine(Path.GetTempPath(), "ui-test-auth-cache");

        Directory.CreateDirectory(_cacheDirectory);
    }

    /// <summary>
    /// Initializes the authenticator by retrieving tenant information from Key Vault.
    /// </summary>
    public async Task InitializeAsync()
    {
        var tenantSecret = await _keyVaultClient.GetSecretAsync("UITest-TenantId");
        _tenantId = tenantSecret.Value.Value;
    }

    /// <summary>
    /// Gets an access token for the specified user type using client credentials flow.
    /// </summary>
    /// <param name="userType">The user type (Admin or User).</param>
    /// <returns>An access token.</returns>
    public async Task<string> GetAccessTokenAsync(UserType userType)
    {
        if (_tenantId == null)
        {
            throw new InvalidOperationException("Authenticator not initialized. Call InitializeAsync first.");
        }

        var credentials = await GetTestCredentialsAsync(userType);

        var app = ConfidentialClientApplicationBuilder
            .Create(credentials.ClientId)
            .WithClientSecret(credentials.ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{_tenantId}"))
            .Build();

        // Request tokens for the app's own API scope that we expose
        // This matches the Application ID URI set in the setup script: api://{clientId}
        var scopes = new[] { $"api://{credentials.ClientId}/.default" };

        try
        {
            var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            throw new InvalidOperationException($"Failed to acquire token for {userType}: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets an access token using device code flow for interactive testing scenarios.
    /// </summary>
    /// <param name="userType">The user type (Admin or User).</param>
    /// <returns>An access token.</returns>
    public async Task<string> GetInteractiveAccessTokenAsync(UserType userType)
    {
        if (_tenantId == null)
        {
            throw new InvalidOperationException("Authenticator not initialized. Call InitializeAsync first.");
        }

        var credentials = await GetTestCredentialsAsync(userType);
        var cacheFilePath = Path.Combine(_cacheDirectory, $"token-cache-{userType}.json");

        var app = PublicClientApplicationBuilder
            .Create(credentials.ClientId)
            .WithAuthority($"https://login.microsoftonline.com/{_tenantId}")
            .WithRedirectUri("http://localhost")
            .Build();

        // Configure token cache
        app.UserTokenCache.SetBeforeAccess(args => LoadTokenCache(args, cacheFilePath));
        app.UserTokenCache.SetAfterAccess(args => SaveTokenCache(args, cacheFilePath));

        var scopes = new[] { "openid", "profile", "email" };
        var accounts = await app.GetAccountsAsync();

        try
        {
            // Try silent acquisition first
            var result = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                .ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException)
        {
            // Use device code flow for interactive authentication
            var result = await app.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
            {
                Console.WriteLine($"\nUI Test Authentication Required for {userType}:");
                Console.WriteLine($"Go to: {deviceCodeResult.VerificationUrl}");
                Console.WriteLine($"Enter code: {deviceCodeResult.UserCode}");
                Console.WriteLine("Waiting for authentication...\n");
                return Task.CompletedTask;
            }).ExecuteAsync();

            return result.AccessToken;
        }
    }

    /// <summary>
    /// Creates an authenticated cookie for web-based testing.
    /// </summary>
    /// <param name="userType">The user type.</param>
    /// <returns>Authentication cookies for the browser context.</returns>
    public async Task<Dictionary<string, string>> GetAuthenticationCookiesAsync(UserType userType)
    {
        var token = await GetAccessTokenAsync(userType);
        var credentials = await GetTestCredentialsAsync(userType);

        // Create a cookie that represents the authenticated session
        // This would typically be done through a proper OIDC flow
        return new Dictionary<string, string>
        {
            ["access_token"] = token,
            ["client_id"] = credentials.ClientId,
            ["user_type"] = userType.ToString()
        };
    }

    private async Task<TestCredentials> GetTestCredentialsAsync(UserType userType)
    {
        var prefix = $"UITest-{userType}";

        var clientIdSecret = await _keyVaultClient.GetSecretAsync($"{prefix}-ClientId");
        var clientSecretSecret = await _keyVaultClient.GetSecretAsync($"{prefix}-ClientSecret");
        var isAdminSecret = await _keyVaultClient.GetSecretAsync($"{prefix}-IsAdmin");

        return new TestCredentials
        {
            ClientId = clientIdSecret.Value.Value,
            ClientSecret = clientSecretSecret.Value.Value,
            IsAdmin = bool.Parse(isAdminSecret.Value.Value)
        };
    }

    private static void LoadTokenCache(TokenCacheNotificationArgs args, string cacheFilePath)
    {
        if (File.Exists(cacheFilePath))
        {
            var data = File.ReadAllBytes(cacheFilePath);
            args.TokenCache.DeserializeMsalV3(data);
        }
    }

    private static void SaveTokenCache(TokenCacheNotificationArgs args, string cacheFilePath)
    {
        if (args.HasStateChanged)
        {
            var data = args.TokenCache.SerializeMsalV3();
            File.WriteAllBytes(cacheFilePath, data);
        }
    }

    private record TestCredentials
    {
        public required string ClientId { get; init; }
        public required string ClientSecret { get; init; }
        public required bool IsAdmin { get; init; }
    }
}

/// <summary>
/// Represents the type of test user.
/// </summary>
public enum UserType
{
    /// <summary>
    /// Admin user with elevated privileges.
    /// </summary>
    Admin,
    /// <summary>
    /// Regular user with standard privileges.
    /// </summary>
    User
}

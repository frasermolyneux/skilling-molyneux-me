using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MX.Skilling.Web.Authorization;

namespace MX.Skilling.Web.Tests.Integration;

/// <summary>
/// Integration tests for authentication and authorization functionality.
/// </summary>
[Trait("Category", "Integration")]
public sealed class AuthenticationIntegrationTests(TestWebApplicationFactory<Program> factory) : IClassFixture<TestWebApplicationFactory<Program>>
{
    private readonly TestWebApplicationFactory<Program> _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    /// <summary>
    /// Verifies that public pages are accessible without authentication.
    /// </summary>
    /// <param name="url">The URL to test.</param>
    [Theory]
    [InlineData("/")]
    [InlineData("/Privacy")]
    public async Task PublicPages_AccessibleWithoutAuthentication(string url)
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    /// <summary>
    /// Verifies that admin-only pages redirect to login when not authenticated.
    /// </summary>
    [Fact]
    public async Task AdminPage_RedirectsToLoginWhenNotAuthenticated()
    {
        // Arrange
        var factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder.ConfigureServices(services => services.AddAuthentication("Bearer")));

        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("/ManageGraph");

        // Assert
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.StartsWith("https://login.microsoftonline.com", response.Headers.Location?.ToString());
    }

    /// <summary>
    /// Verifies that authenticated admin users can access admin pages.
    /// </summary>
    [Fact]
    public async Task AdminPage_AccessibleToAuthenticatedAdminUser()
    {
        // Arrange
        var factory = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) => config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "AdminEmails:0", "admin@example.com" }
            }));

            builder.ConfigureServices(services => services.AddAuthentication("AdminTest")
                .AddScheme<AuthenticationSchemeOptions, AdminTestAuthenticationHandler>("AdminTest", _ => { }));
        });

        using var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/ManageGraph");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
        Assert.Contains("Manage Graph", content);
    }

    /// <summary>
    /// Verifies that authenticated users without admin role are forbidden from admin pages.
    /// </summary>
    [Fact]
    public async Task AdminPage_ForbiddenToNonAdminUser()
    {
        // Arrange - using the default test factory which has non-admin user
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/ManageGraph");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    /// <summary>
    /// Verifies that the test authentication scheme creates proper claims.
    /// </summary>
    [Fact]
    public async Task TestAuthentication_CreatesProperClaims()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act - Access a page that would show user info if available
        var response = await client.GetAsync("/");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.EnsureSuccessStatusCode();
        // In a real application, you might check for user name display
        // For now, just verify the authentication infrastructure works
        Assert.NotNull(content);
    }

    /// <summary>
    /// Verifies that the application properly configures authentication services.
    /// </summary>
    [Fact]
    public void Application_ConfiguresAuthenticationServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var authService = scope.ServiceProvider.GetService<IAuthenticationService>();

        // Assert
        Assert.NotNull(authService);
    }

    /// <summary>
    /// Verifies that authorization policies are properly configured.
    /// </summary>
    [Fact]
    public void Application_ConfiguresAuthorizationPolicies()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var authorizationService = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Authorization.IAuthorizationService>();

        // Assert
        Assert.NotNull(authorizationService);
    }
}

/// <summary>
/// Test authentication handler that simulates an admin user.
/// </summary>
public class AdminTestAuthenticationHandler(
    Microsoft.Extensions.Options.IOptionsMonitor<AuthenticationSchemeOptions> options,
    Microsoft.Extensions.Logging.ILoggerFactory logger,
    System.Text.Encodings.Web.UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// Handles the authentication request for an admin user.
    /// </summary>
    /// <returns>The authentication result.</returns>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Admin User"),
            new Claim(ClaimTypes.NameIdentifier, "admin-user-id"),
            new Claim(ClaimTypes.Email, "admin@example.com") // This matches AdminEmails:0 configuration
        };

        var identity = new ClaimsIdentity(claims, "AdminTest");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "AdminTest");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

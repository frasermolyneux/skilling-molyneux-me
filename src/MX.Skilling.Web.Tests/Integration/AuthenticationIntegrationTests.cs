using System.Net;
using Microsoft.Extensions.DependencyInjection;

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
    /// Verifies that admin-only pages are forbidden to non-admin users.
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
    /// Verifies that the application properly configures authentication services.
    /// </summary>
    [Fact]
    public void Application_ConfiguresAuthenticationServices()
    {
        // Arrange & Act
        using var scope = _factory.Services.CreateScope();
        var authService = scope.ServiceProvider.GetService<Microsoft.AspNetCore.Authentication.IAuthenticationService>();

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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MX.Skilling.Web.Extensions;

namespace MX.Skilling.Web.Tests.Extensions;

/// <summary>
/// Unit tests for KeyVaultExtensions functionality.
/// </summary>
[Trait("Category", "Unit")]
public sealed class KeyVaultExtensionsTests
{
    /// <summary>
    /// Verifies that Key Vault configuration is skipped in testing environments.
    /// </summary>
    [Fact]
    public void AddKeyVaultConfiguration_InTestingEnvironment_SkipsKeyVaultConfiguration()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Simulate testing environment configuration
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "KeyVaultUri", "https://test-vault.vault.azure.net/" }
        });

        // Set environment to Testing
        builder.Environment.EnvironmentName = "Testing";

        // Act
        builder.AddKeyVaultConfiguration();

        // Assert
        // Verify that DefaultAzureCredential was not registered
        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var credential = scope.ServiceProvider.GetService<Azure.Identity.DefaultAzureCredential>();

        Assert.Null(credential);
    }

    /// <summary>
    /// Verifies that Key Vault configuration is skipped when no URI is provided.
    /// </summary>
    [Fact]
    public void AddKeyVaultConfiguration_WithoutKeyVaultUri_SkipsConfiguration()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // No KeyVaultUri in configuration
        builder.Environment.EnvironmentName = "Development";

        // Act
        builder.AddKeyVaultConfiguration();

        // Assert
        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var credential = scope.ServiceProvider.GetService<Azure.Identity.DefaultAzureCredential>();

        Assert.Null(credential);
    }

    /// <summary>
    /// Verifies that Key Vault configuration is skipped when URI is empty.
    /// </summary>
    [Fact]
    public void AddKeyVaultConfiguration_WithEmptyKeyVaultUri_SkipsConfiguration()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Empty KeyVaultUri in configuration
        builder.Configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "KeyVaultUri", "" }
        });
        builder.Environment.EnvironmentName = "Development";

        // Act
        builder.AddKeyVaultConfiguration();

        // Assert
        var app = builder.Build();
        using var scope = app.Services.CreateScope();
        var credential = scope.ServiceProvider.GetService<Azure.Identity.DefaultAzureCredential>();

        Assert.Null(credential);
    }
}

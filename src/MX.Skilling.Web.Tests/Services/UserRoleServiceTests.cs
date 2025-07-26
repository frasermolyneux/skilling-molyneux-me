using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using MX.Skilling.Web.Services;

namespace MX.Skilling.Web.Tests.Services;

/// <summary>
/// Unit tests for the <see cref="UserRoleService"/> class.
/// </summary>
[Trait("Category", "Unit")]
public class UserRoleServiceTests
{
    /// <summary>
    /// Verifies that admin role check works with comma-separated app setting configuration.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_WithCommaSeparatedAppSetting_ReturnsTrue()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();
        var testUserEmail = "test@example.com";
        var adminEmails = $"admin1@example.com,{testUserEmail}, admin3@example.com";

        var configurationData = new Dictionary<string, string?>
        {
            ["AdminEmails"] = adminEmails
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, testUserEmail)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that admin role check works with array configuration from appsettings.json.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_WithArrayConfiguration_ReturnsTrue()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();
        var testUserEmail = "test@example.com";

        var configurationData = new Dictionary<string, string?>
        {
            ["AdminEmails:0"] = "admin1@example.com",
            ["AdminEmails:1"] = testUserEmail,
            ["AdminEmails:2"] = "admin3@example.com"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, testUserEmail)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that admin role check returns false when user is not in the admin list.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_UserNotInAdminList_ReturnsFalse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();

        var configurationData = new Dictionary<string, string?>
        {
            ["AdminEmails"] = "admin1@example.com,admin2@example.com,admin3@example.com"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "different@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that admin role check returns false for unauthenticated users.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_UnauthenticatedUser_ReturnsFalse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();
        var configuration = new ConfigurationBuilder().Build();

        var service = new UserRoleService(mockLogger.Object, configuration);
        var user = new ClaimsPrincipal(); // No authenticated identity

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that admin role check returns false when no configuration is provided.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_NoConfiguration_ReturnsFalse()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();
        var configuration = new ConfigurationBuilder().Build(); // Empty configuration

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "test@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Verifies that email comparison is case-insensitive.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_CaseInsensitive_ReturnsTrue()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();

        var configurationData = new Dictionary<string, string?>
        {
            ["AdminEmails"] = "ADMIN@EXAMPLE.COM"
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, "admin@example.com")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that admin role check works with preferred_username claim when email claim is not available.
    /// </summary>
    [Fact]
    public async Task IsUserInAdminRoleAsync_WithPreferredUsernameClaim_ReturnsTrue()
    {
        // Arrange
        var mockLogger = new Mock<ILogger<UserRoleService>>();
        var testUserEmail = "test@example.com";

        var configurationData = new Dictionary<string, string?>
        {
            ["AdminEmails"] = testUserEmail
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurationData)
            .Build();

        var service = new UserRoleService(mockLogger.Object, configuration);

        var claims = new List<Claim>
        {
            new("preferred_username", testUserEmail)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Act
        var result = await service.IsUserInAdminRoleAsync(user);

        // Assert
        Assert.True(result);
    }
}

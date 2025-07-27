using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;

namespace MX.Skilling.Web.Extensions;

/// <summary>
/// Extension methods for configuring Key Vault integration.
/// </summary>
public static class KeyVaultExtensions
{
    /// <summary>
    /// Adds Azure Key Vault configuration provider to the configuration builder.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <returns>The web application builder for method chaining.</returns>
    public static WebApplicationBuilder AddKeyVaultConfiguration(this WebApplicationBuilder builder)
    {
        var keyVaultUri = builder.Configuration["KeyVaultUri"];

        // Skip Key Vault configuration in testing environments to avoid Azure credential requirements
        if (!string.IsNullOrEmpty(keyVaultUri) && !builder.Environment.IsEnvironment("Testing"))
        {
            // Use managed identity in Azure, default Azure credential locally
            var credential = new DefaultAzureCredential();

            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultUri),
                credential);

            builder.Services.AddSingleton(credential);
        }

        return builder;
    }
}

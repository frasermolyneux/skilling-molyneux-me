using Azure.Identity;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MX.Skilling.Web.Authorization;
using MX.Skilling.Web.Configuration;
using MX.Skilling.Web.Services;

namespace MX.Skilling.Web.Extensions;

/// <summary>
/// Extension methods for configuring authentication and authorization services.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds Azure AD authentication and authorization services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddAzureAdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure Azure AD options
        services.Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.SectionName));

        // Add authentication services
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = "Cookies";
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
        .AddCookie("Cookies")
        .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                var azureAdSection = configuration.GetSection(AzureAdOptions.SectionName);
                options.Authority = $"{azureAdSection["Instance"]}{azureAdSection["TenantId"]}/v2.0";
                options.ClientId = azureAdSection["ClientId"];
                options.ClientSecret = azureAdSection["ClientSecret"];
                options.ResponseType = OpenIdConnectResponseType.Code;
                options.CallbackPath = azureAdSection["CallbackPath"];
                options.UsePkce = true;
                options.SaveTokens = true;

                // Request additional scopes
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Map claims
                options.TokenValidationParameters.NameClaimType = "preferred_username";
                options.TokenValidationParameters.RoleClaimType = "roles";
            });

        // Add authorization services
        services.AddAuthorization(options =>
            options.AddPolicy(AuthorizationConstants.AdminPolicy, policy =>
                policy.Requirements.Add(new AdminRoleRequirement())));

        // Register custom services
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IAuthorizationHandler, AdminRoleHandler>();

        return services;
    }
}

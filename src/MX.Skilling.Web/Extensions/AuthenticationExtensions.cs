using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
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
        var enableUITesting = configuration.GetValue<bool>("UITesting:Enabled");

        if (enableUITesting)
        {
            return AddUITestAuthentication(services, configuration);
        }

        return AddProductionAuthentication(services, configuration);
    }

    private static IServiceCollection AddProductionAuthentication(IServiceCollection services, IConfiguration configuration)
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

        return AddCommonServices(services);
    }

    private static IServiceCollection AddUITestAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var tenantId = configuration["UITesting:TenantId"];

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = $"https://login.microsoftonline.com/{tenantId}/v2.0";

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                // Accept both v1.0 and v2.0 endpoints for UI testing
                ValidIssuers = new[]
                {
                    $"https://login.microsoftonline.com/{tenantId}/v2.0",
                    $"https://sts.windows.net/{tenantId}/"
                },
                // Custom audience validation for UI test app registrations
                AudienceValidator = (audiences, securityToken, validationParameters) =>
                    // Accept any audience that starts with "api://" for UI testing
                    // This allows tokens from any of our test app registrations
                    audiences.Any(aud => aud.StartsWith("api://", StringComparison.OrdinalIgnoreCase)),
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                NameClaimType = "preferred_username",
                RoleClaimType = "roles",
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    logger.LogWarning("JWT Authentication failed: {Exception}", context.Exception);
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                    logger.LogInformation("JWT Token validated for: {User}", context.Principal?.Identity?.Name);
                    return Task.CompletedTask;
                }
            };
        });

        return AddUITestCommonServices(services);
    }

    private static IServiceCollection AddCommonServices(IServiceCollection services)
    {
        // Add authorization services
        services.AddAuthorization(options =>
            options.AddPolicy(AuthorizationConstants.AdminPolicy, policy =>
                policy.Requirements.Add(new AdminRoleRequirement())));

        // Register custom services
        services.AddScoped<IUserRoleService, UserRoleService>();
        services.AddScoped<IAuthorizationHandler, AdminRoleHandler>();

        return services;
    }

    private static IServiceCollection AddUITestCommonServices(IServiceCollection services)
    {
        // Add authorization services
        services.AddAuthorization(options =>
            options.AddPolicy(AuthorizationConstants.AdminPolicy, policy =>
                policy.Requirements.Add(new AdminRoleRequirement())));

        // Register custom services - use UI test version that accepts app tokens
        services.AddScoped<IUserRoleService, UITestUserRoleService>();
        services.AddScoped<IAuthorizationHandler, AdminRoleHandler>();

        return services;
    }
}

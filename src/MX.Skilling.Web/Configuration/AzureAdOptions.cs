using System.ComponentModel.DataAnnotations;

namespace MX.Skilling.Web.Configuration;

/// <summary>
/// Configuration options for Azure AD authentication.
/// </summary>
public class AzureAdOptions
{
    /// <summary>
    /// Configuration section name for Azure AD settings.
    /// </summary>
    public const string SectionName = "AzureAd";

    /// <summary>
    /// The Azure AD instance URL (e.g., https://login.microsoftonline.com/).
    /// </summary>
    [Required]
    public required string Instance { get; init; }

    /// <summary>
    /// The Azure AD tenant ID.
    /// </summary>
    [Required]
    public required string TenantId { get; init; }

    /// <summary>
    /// The Azure AD domain (e.g., contoso.onmicrosoft.com).
    /// </summary>
    [Required]
    public required string Domain { get; init; }

    /// <summary>
    /// The application client ID registered in Azure AD.
    /// </summary>
    [Required]
    public required string ClientId { get; init; }

    /// <summary>
    /// The application client secret registered in Azure AD.
    /// </summary>
    [Required]
    public required string ClientSecret { get; init; }

    /// <summary>
    /// The callback path for OpenID Connect authentication.
    /// </summary>
    [Required]
    public required string CallbackPath { get; init; }
}

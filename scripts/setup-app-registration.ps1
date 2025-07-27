#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Creates or updates an Azure AD app registration for the MX.Skilling application.

.DESCRIPTION
    This script creates an Azure AD app registration with the necessary permissions
    and configures redirect URIs for the web application. It also assigns admin users
    to the application.

.PARAMETER EnvironmentName
    The environment name (e.g., 'dev', 'staging', 'prod')

.PARAMETER WebAppUrl
    The URL of the deployed web application

.PARAMETER KeyVaultName
    The name of the Key Vault to store secrets

.EXAMPLE
    ./setup-app-registration.ps1 -EnvironmentName "dev" -WebAppUrl "https://app-skilling-dev-abc123.azurewebsites.net" -KeyVaultName "kv-skilling-dev-abc123"
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$EnvironmentName,

    [Parameter(Mandatory = $true)]
    [string]$WebAppUrl,

    [Parameter(Mandatory = $true)]
    [string]$KeyVaultName
)

# Set error action preference
$ErrorActionPreference = "Stop"

Write-Host "Setting up Azure AD App Registration for environment: $EnvironmentName" -ForegroundColor Green

# Validate prerequisites
Write-Host "Validating prerequisites..." -ForegroundColor Yellow

# Check if Azure CLI is available
try {
    az --version | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI not found or not working properly"
    }
    Write-Host "✓ Azure CLI is available" -ForegroundColor Green
}
catch {
    Write-Error "Azure CLI is required but not found. Please install Azure CLI and try again."
    exit 1
}

# Check if user is logged in to Azure CLI
try {
    $currentAccount = az account show --query "user.name" -o tsv
    if ($LASTEXITCODE -ne 0 -or -not $currentAccount) {
        throw "Not logged in to Azure CLI"
    }
    Write-Host "✓ Logged in to Azure CLI as: $currentAccount" -ForegroundColor Green
}
catch {
    Write-Error "You must be logged in to Azure CLI. Please run 'az login' and try again."
    exit 1
}

# App registration details
$AppName = "MX.Skilling-$EnvironmentName"
$RedirectUri = "$($WebAppUrl.TrimEnd('/'))/signin-oidc"

# For dev environment, also include localhost for local development
$RedirectUris = @($RedirectUri)
if ($EnvironmentName -eq "dev") {
    $RedirectUris += "https://localhost:7053/signin-oidc"
    Write-Host "Adding localhost redirect URI for dev environment" -ForegroundColor Cyan
}

try {
    # Check if app registration already exists
    Write-Host "Checking if app registration '$AppName' exists..." -ForegroundColor Yellow
    $existingAppJson = az ad app list --display-name $AppName --query "[0]" -o json
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to check existing app registration. Azure CLI returned exit code: $LASTEXITCODE"
    }

    $existingApp = $existingAppJson | ConvertFrom-Json

    if ($existingApp) {
        Write-Host "App registration '$AppName' already exists with ID: $($existingApp.appId)" -ForegroundColor Yellow
        $appId = $existingApp.appId
        $objectId = $existingApp.id

        # Validate that we have the required properties
        if (-not $appId -or -not $objectId) {
            throw "Existing app registration found but missing required properties (appId: '$appId', objectId: '$objectId')"
        }
    }
    else {
        # Create the app registration
        Write-Host "Creating app registration '$AppName'..." -ForegroundColor Yellow
        $redirectUriArgs = @()
        foreach ($uri in $RedirectUris) {
            $redirectUriArgs += $uri
        }

        $appRegistrationJson = az ad app create --display-name $AppName --web-redirect-uris @redirectUriArgs --enable-id-token-issuance true --query "{appId: appId, id: id}" -o json
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create app registration. Azure CLI returned exit code: $LASTEXITCODE"
        }

        $appRegistration = $appRegistrationJson | ConvertFrom-Json
        if (-not $appRegistration) {
            throw "Failed to parse app registration response from Azure CLI"
        }

        $appId = $appRegistration.appId
        $objectId = $appRegistration.id

        # Validate that we have the required properties
        if (-not $appId -or -not $objectId) {
            throw "App registration created but missing required properties (appId: '$appId', objectId: '$objectId')"
        }

        Write-Host "Created app registration with ID: $appId" -ForegroundColor Green
    }

    # Update redirect URIs if needed
    Write-Host "Updating redirect URIs..." -ForegroundColor Yellow
    $redirectUriArgs = @()
    foreach ($uri in $RedirectUris) {
        $redirectUriArgs += $uri
    }

    az ad app update --id $objectId --web-redirect-uris @redirectUriArgs
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to update redirect URIs. Azure CLI returned exit code: $LASTEXITCODE"
    }

    # Create or reset client secret
    Write-Host "Creating client secret..." -ForegroundColor Yellow
    $secretName = "GitHub-Actions-Secret-$(Get-Date -Format 'yyyyMMdd')"
    $clientSecret = az ad app credential reset --id $objectId --display-name $secretName --query "password" -o tsv
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create client secret. Azure CLI returned exit code: $LASTEXITCODE"
    }

    if (-not $clientSecret -or $clientSecret.Trim() -eq "") {
        throw "Client secret creation succeeded but returned empty value"
    }

    Write-Host "Client secret created successfully" -ForegroundColor Green

    # Store secrets in Key Vault
    Write-Host "Storing secrets in Key Vault '$KeyVaultName'..." -ForegroundColor Yellow

    az keyvault secret set --vault-name $KeyVaultName --name "AzureAd--ClientId" --value $appId --only-show-errors | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to store ClientId in Key Vault. Azure CLI returned exit code: $LASTEXITCODE"
    }

    az keyvault secret set --vault-name $KeyVaultName --name "AzureAd--ClientSecret" --value $clientSecret --only-show-errors | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to store ClientSecret in Key Vault. Azure CLI returned exit code: $LASTEXITCODE"
    }

    Write-Host "Successfully configured app registration and stored secrets in Key Vault" -ForegroundColor Green

    # Output summary
    Write-Host "`n=== Configuration Summary ===" -ForegroundColor Cyan
    Write-Host "App Registration Name: $AppName" -ForegroundColor White
    Write-Host "App Registration ID: $appId" -ForegroundColor White
    Write-Host "Redirect URIs: $($RedirectUris -join ', ')" -ForegroundColor White
    Write-Host "Key Vault: $KeyVaultName" -ForegroundColor White
}
catch {
    Write-Host "❌ Error occurred during app registration setup:" -ForegroundColor Red
    Write-Host "Environment: $EnvironmentName" -ForegroundColor Red
    Write-Host "App Name: $AppName" -ForegroundColor Red
    Write-Host "Error Message: $($_.Exception.Message)" -ForegroundColor Red

    if ($_.Exception.InnerException) {
        Write-Host "Inner Exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
    }

    Write-Error "Failed to set up app registration: $_"
    exit 1
}

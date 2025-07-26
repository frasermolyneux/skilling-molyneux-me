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
    $existingApp = az ad app list --display-name $AppName --query "[0]" -o json | ConvertFrom-Json

    if ($existingApp) {
        Write-Host "App registration '$AppName' already exists with ID: $($existingApp.appId)" -ForegroundColor Yellow
        $appId = $existingApp.appId
        $objectId = $existingApp.id
    }
    else {
        # Create the app registration
        Write-Host "Creating app registration '$AppName'..." -ForegroundColor Yellow
        $redirectUriString = ($RedirectUris | ForEach-Object { "`"$_`"" }) -join " "
        $createCommand = "az ad app create --display-name `"$AppName`" --web-redirect-uris $redirectUriString --enable-id-token-issuance true --query `"{appId: appId, id: id}`" -o json"
        $appRegistration = Invoke-Expression $createCommand | ConvertFrom-Json

        $appId = $appRegistration.appId
        $objectId = $appRegistration.id
        Write-Host "Created app registration with ID: $appId" -ForegroundColor Green
    }

    # Update redirect URIs if needed
    Write-Host "Updating redirect URIs..." -ForegroundColor Yellow
    $redirectUriString = ($RedirectUris | ForEach-Object { "`"$_`"" }) -join " "
    $updateCommand = "az ad app update --id $objectId --web-redirect-uris $redirectUriString"
    Invoke-Expression $updateCommand | Out-Null

    # Create or reset client secret
    Write-Host "Creating client secret..." -ForegroundColor Yellow
    $secretName = "GitHub-Actions-Secret-$(Get-Date -Format 'yyyyMMdd')"
    $clientSecret = az ad app credential reset --id $objectId --display-name $secretName --query "password" -o tsv

    Write-Host "Client secret created successfully" -ForegroundColor Green

    # Store secrets in Key Vault
    Write-Host "Storing secrets in Key Vault '$KeyVaultName'..." -ForegroundColor Yellow

    az keyvault secret set --vault-name $KeyVaultName --name "AzureAd--ClientId" --value $appId --only-show-errors | Out-Null
    az keyvault secret set --vault-name $KeyVaultName --name "AzureAd--ClientSecret" --value $clientSecret --only-show-errors | Out-Null

    Write-Host "Successfully configured app registration and stored secrets in Key Vault" -ForegroundColor Green

    # Output summary
    Write-Host "`n=== Configuration Summary ===" -ForegroundColor Cyan
    Write-Host "App Registration Name: $AppName" -ForegroundColor White
    Write-Host "App Registration ID: $appId" -ForegroundColor White
    Write-Host "Redirect URIs: $($RedirectUris -join ', ')" -ForegroundColor White
    Write-Host "Key Vault: $KeyVaultName" -ForegroundColor White
}
catch {
    Write-Error "Failed to set up app registration: $_"
    exit 1
}

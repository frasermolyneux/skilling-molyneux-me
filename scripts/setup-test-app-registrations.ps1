#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Creates test app registrations for UI testing with real Entra ID authentication.

.DESCRIPTION
    This script creates dedicated app registrations for UI testing purposes,
    with appropriate permissions and credentials stored in Key Vault.

.PARAMETER EnvironmentName
    The environment name (e.g., 'dev', 'staging', 'prod')

.PARAMETER KeyVaultName
    The name of the Key Vault to store test credentials

.PARAMETER CreateTestUsers
    Whether to create test user accounts as well

.EXAMPLE
    ./setup-test-app-registrations.ps1 -EnvironmentName "dev" -KeyVaultName "kv-skilling-dev-e3r7w4" -CreateTestUsers
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$EnvironmentName,

    [Parameter(Mandatory = $true)]
    [string]$KeyVaultName,

    [Parameter()]
    [switch]$CreateTestUsers
)

$ErrorActionPreference = "Stop"

Write-Host "Setting up Test App Registrations for environment: $EnvironmentName" -ForegroundColor Green

# Validate prerequisites
Write-Host "Validating prerequisites..." -ForegroundColor Yellow

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

function New-TestAppRegistration {
    param(
        [string]$AppName,
        [string]$UserType,
        [bool]$IsAdmin = $false
    )

    Write-Host "Creating test app registration: $AppName" -ForegroundColor Yellow

    # Check if app registration already exists
    $existingAppJson = az ad app list --display-name $AppName --query "[0]" -o json
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to check existing app registration"
    }

    $existingApp = $existingAppJson | ConvertFrom-Json

    if ($existingApp) {
        Write-Host "App registration '$AppName' already exists, updating..." -ForegroundColor Cyan
        $appId = $existingApp.appId
        $objectId = $existingApp.id

        # Ensure service principal exists for existing app registration
        Write-Host "Checking service principal for existing app..." -ForegroundColor Cyan
        $existingSpJson = az ad sp list --filter "appId eq '$appId'" --query "[0]" -o json
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to check existing service principal"
        }

        $existingSp = $existingSpJson | ConvertFrom-Json
        if (-not $existingSp) {
            Write-Host "Creating missing service principal..." -ForegroundColor Cyan
            az ad sp create --id $appId | Out-Null
            if ($LASTEXITCODE -ne 0) {
                throw "Failed to create service principal for existing app"
            }
            Write-Host "✓ Service principal created successfully" -ForegroundColor Green
        }
        else {
            Write-Host "✓ Service principal already exists" -ForegroundColor Green
        }
    }
    else {
        Write-Host "Creating new app registration '$AppName'..." -ForegroundColor Cyan

        # Create a temporary file for the required resource accesses JSON
        $tempJsonFile = [System.IO.Path]::GetTempFileName()
        $requiredResourceAccesses = @"
[{
    "resourceAppId": "00000003-0000-0000-c000-000000000000",
    "resourceAccess": [
        {
            "id": "e1fe6dd8-ba31-4d61-89e7-88639da4683d",
            "type": "Scope"
        },
        {
            "id": "37f7f235-527c-4136-accd-4a02d197296e",
            "type": "Scope"
        },
        {
            "id": "14dad69e-099b-42c9-810b-d002981feec1",
            "type": "Scope"
        }
    ]
}]
"@

        try {
            # Write JSON to temporary file
            $requiredResourceAccesses | Out-File -FilePath $tempJsonFile -Encoding utf8

            $appRegistrationJson = az ad app create --display-name $AppName `
                --sign-in-audience "AzureADMyOrg" `
                --required-resource-accesses "@$tempJsonFile" `
                --web-redirect-uris "https://login.microsoftonline.com/common/oauth2/nativeclient" `
                --enable-id-token-issuance true `
                --enable-access-token-issuance true
        }
        finally {
            # Clean up temporary file
            if (Test-Path $tempJsonFile) {
                Remove-Item $tempJsonFile -Force
            }
        }

        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create app registration"
        }

        $appRegistration = $appRegistrationJson | ConvertFrom-Json
        $appId = $appRegistration.appId
        $objectId = $appRegistration.id

        # Set Application ID URI to expose an API
        Write-Host "Setting Application ID URI for $AppName..." -ForegroundColor Cyan
        az ad app update --id $objectId --identifier-uris "api://$appId" | Out-Null

        if ($LASTEXITCODE -ne 0) {
            throw "Failed to set Application ID URI"
        }

        # Expose a default API scope
        Write-Host "Exposing API scope for $AppName..." -ForegroundColor Cyan

        # Generate a new GUID for the scope
        $scopeId = [System.Guid]::NewGuid().ToString()

        # First, initialize the api section if it doesn't exist
        az ad app update --id $objectId --set api="{}" | Out-Null
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Could not initialize API section, continuing..."
        }

        # Create the scope configuration using direct property assignment
        $tempScopeFile = [System.IO.Path]::GetTempFileName()
        try {
            $apiScopeJson = @"
[
    {
        "id": "$scopeId",
        "adminConsentDisplayName": "Access UITest API",
        "adminConsentDescription": "Allow access to UITest API for testing purposes",
        "userConsentDisplayName": "Access UITest API",
        "userConsentDescription": "Allow access to UITest API for testing purposes",
        "value": "access_as_user",
        "type": "User",
        "isEnabled": true
    }
]
"@
            $apiScopeJson | Out-File -FilePath $tempScopeFile -Encoding utf8 -NoNewline

            # Try multiple approaches in case one fails
            $updateSuccess = $false

            # Approach 1: Direct property update
            try {
                az ad app update --id $objectId --set api.oauth2PermissionScopes="@$tempScopeFile" | Out-Null
                if ($LASTEXITCODE -eq 0) {
                    $updateSuccess = $true
                }
            }
            catch {
                Write-Warning "Direct property update failed, trying alternative approach..."
            }

            # Approach 2: Full API object update if direct property failed
            if (-not $updateSuccess) {
                $fullApiJson = @"
{
    "oauth2PermissionScopes": [
        {
            "id": "$scopeId",
            "adminConsentDisplayName": "Access UITest API",
            "adminConsentDescription": "Allow access to UITest API for testing purposes",
            "userConsentDisplayName": "Access UITest API",
            "userConsentDescription": "Allow access to UITest API for testing purposes",
            "value": "access_as_user",
            "type": "User",
            "isEnabled": true
        }
    ]
}
"@
                Remove-Item $tempScopeFile -Force
                $tempScopeFile = [System.IO.Path]::GetTempFileName()
                $fullApiJson | Out-File -FilePath $tempScopeFile -Encoding utf8 -NoNewline

                az ad app update --id $objectId --set api="@$tempScopeFile" | Out-Null
                if ($LASTEXITCODE -ne 0) {
                    Write-Warning "API scope configuration may not have been set correctly, but continuing..."
                }
            }
        }
        finally {
            if (Test-Path $tempScopeFile) {
                Remove-Item $tempScopeFile -Force
            }
        }

        Write-Host "Created app registration with ID: $appId" -ForegroundColor Green
    }

    # Create service principal for the app registration
    Write-Host "Creating service principal for $AppName..." -ForegroundColor Cyan
    $existingSpJson = az ad sp list --filter "appId eq '$appId'" --query "[0]" -o json
    if ($LASTEXITCODE -ne 0) {
        throw "Failed to check existing service principal"
    }

    $existingSp = $existingSpJson | ConvertFrom-Json
    if (-not $existingSp) {
        az ad sp create --id $appId | Out-Null
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to create service principal"
        }
        Write-Host "✓ Service principal created successfully" -ForegroundColor Green
    }
    else {
        Write-Host "✓ Service principal already exists" -ForegroundColor Green
    }

    # Create client secret
    Write-Host "Creating client secret for $AppName..." -ForegroundColor Yellow
    $secretName = "UITest-Secret-$(Get-Date -Format 'yyyyMMdd')"
    $clientSecretJson = az ad app credential reset --id $objectId --display-name $secretName --years 1

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to create client secret"
    }

    $clientSecret = ($clientSecretJson | ConvertFrom-Json).password

    # Store credentials in Key Vault
    $kvSecretPrefix = "UITest-$UserType"

    Write-Host "Storing credentials in Key Vault..." -ForegroundColor Yellow

    az keyvault secret set --vault-name $KeyVaultName `
        --name "$kvSecretPrefix-ClientId" `
        --value $appId --only-show-errors | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to store ClientId in Key Vault"
    }

    az keyvault secret set --vault-name $KeyVaultName `
        --name "$kvSecretPrefix-ClientSecret" `
        --value $clientSecret --only-show-errors | Out-Null

    if ($LASTEXITCODE -ne 0) {
        throw "Failed to store ClientSecret in Key Vault"
    }

    # Store additional metadata
    az keyvault secret set --vault-name $KeyVaultName `
        --name "$kvSecretPrefix-IsAdmin" `
        --value $IsAdmin.ToString().ToLower() --only-show-errors | Out-Null

    # Grant admin consent for API permissions
    Write-Host "Granting admin consent for API permissions..." -ForegroundColor Yellow
    az ad app permission admin-consent --id $appId | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Admin consent granted successfully" -ForegroundColor Green
    }
    else {
        Write-Warning "Failed to grant admin consent automatically. Please grant manually in Azure Portal."
    }

    Write-Host "✓ Test app registration '$AppName' configured successfully" -ForegroundColor Green

    return @{
        AppId        = $appId
        ObjectId     = $objectId
        SecretPrefix = $kvSecretPrefix
    }
}

try {
    # Create test app registrations
    Write-Host "`n=== Creating Test App Registrations ===" -ForegroundColor Cyan

    # Admin test app
    $adminApp = New-TestAppRegistration -AppName "MX.Skilling.UITest.Admin-$EnvironmentName" -UserType "Admin" -IsAdmin $true

    # Regular user test app
    $userApp = New-TestAppRegistration -AppName "MX.Skilling.UITest.User-$EnvironmentName" -UserType "User" -IsAdmin $false

    # Store tenant information
    $tenantId = (az account show --query "tenantId" -o tsv)
    az keyvault secret set --vault-name $KeyVaultName `
        --name "UITest-TenantId" `
        --value $tenantId --only-show-errors | Out-Null

    Write-Host "`n=== Configuration Summary ===" -ForegroundColor Cyan
    Write-Host "Admin Test App ID: $($adminApp.AppId)" -ForegroundColor White
    Write-Host "User Test App ID: $($userApp.AppId)" -ForegroundColor White
    Write-Host "Tenant ID: $tenantId" -ForegroundColor White
    Write-Host "Key Vault: $KeyVaultName" -ForegroundColor White

    Write-Host "`n✓ Test app registrations setup completed successfully!" -ForegroundColor Green

    if ($CreateTestUsers) {
        Write-Host "`nNote: Test user creation not implemented yet. Use existing users or create manually." -ForegroundColor Yellow
    }

    Write-Host "`nNext steps:" -ForegroundColor Cyan
    Write-Host "1. Your test app registrations are ready to use!" -ForegroundColor White
    Write-Host "2. Run your UI tests with real Entra ID authentication" -ForegroundColor White
    Write-Host "3. If authentication issues persist, verify admin consent in Azure Portal" -ForegroundColor White
}
catch {
    Write-Error "Failed to setup test app registrations: $_"
    exit 1
}

---
applyTo: '**/*.bicep, **/main.parameters.json'
---

# Bicep Infrastructure Guidelines

## Core Principles

### Infrastructure as Code Standards
- **Use Bicep over ARM templates** - leverage simplified syntax and type safety
- **Organize resources logically** - group related resources in modules
- **Use consistent naming conventions** - follow Azure naming conventions
- **Implement proper tagging strategy** - enable cost tracking and resource management
- **Version control all infrastructure** - treat infrastructure as code

### Security First
- **Use managed identities** instead of service principals when possible
- **Implement least privilege access** - grant minimum required permissions
- **Use Key Vault for secrets** - never hardcode sensitive values
- **Enable diagnostic logging** for all resources
- **Use private endpoints** for sensitive resources

## File Organization

### Main Template Structure
```bicep
// main.bicep
targetScope = 'resourceGroup'

@description('Environment name (e.g., dev, staging, prod)')
param environmentName string

@description('Location for all resources')
param location string = resourceGroup().location

@description('Unique suffix for resource names')
param resourceToken string = uniqueString(resourceGroup().id)

// Resource definitions
module appService 'modules/app-service.bicep' = {
  name: 'app-service-deployment'
  params: {
    environmentName: environmentName
    location: location
    resourceToken: resourceToken
  }
}
```

### Module Structure
- **Create focused modules** - one service type per module
- **Use clear parameter definitions** with descriptions and types
- **Return outputs** for resources that other modules depend on
- **Include resource dependencies** explicitly

```bicep
// modules/app-service.bicep
@description('Environment name')
param environmentName string

@description('Location for resources')
param location string

@description('Resource naming token')
param resourceToken string

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: 'asp-skilling-${resourceToken}'
  location: location
  tags: {
    Environment: environmentName
    'azd-env-name': environmentName
  }
  sku: {
    name: 'B1'
    capacity: 1
  }
}

output appServicePlanId string = appServicePlan.id
```

## Best Practices

### Naming Conventions
- **Use consistent prefixes** - Cloud Adoption Framework resource type abbreviations (e.g., `asp-`, `app-`, `kv-`, `id-`)
- **Include environment indicators** - use environment name in resource names
- **Use resource tokens** - ensure uniqueness with `uniqueString()`
- **Follow Azure naming rules** - respect length limits and allowed characters

```bicep
var resourceNames = {
  resourceGroup: 'rg-skilling-${environmentName}'
  keyVault: 'kv-skilling-${environmentName}-${resourceToken}'
  appService: 'app-skilling-${environmentName}-${resourceToken}'
  appServicePlan: 'asp-skilling-${environmentName}-${resourceToken}'
  managedIdentity: 'id-skilling-${environmentName}-${resourceToken}'
  storageAccount: 'st${replace(resourceToken, '-', '')}'
}
```

### Parameter Management
- **Use parameter files** for environment-specific values
- **Provide default values** when appropriate
- **Use allowed values** for constrained parameters
- **Include parameter descriptions** for clarity

```bicep
@description('The SKU of the App Service Plan')
@allowed(['B1', 'B2', 'S1', 'S2', 'P1v3', 'P2v3'])
param appServicePlanSku string = 'B1'

@description('Enable Application Insights')
param enableApplicationInsights bool = true

@minLength(3)
@maxLength(24)
@description('Storage account name')
param storageAccountName string
```

### Tagging Strategy
- **Apply consistent tags** to all resources
- **Tag with environment** for resource management
- **Add azd-env-name tag** for Azure Developer CLI compatibility

```bicep
var commonTags = {
  Environment: environmentName
  Project: 'MX.Skilling'
  ManagedBy: 'Bicep'
  'azd-env-name': environmentName
}

resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: resourceNames.appService
  location: location
  tags: commonTags
  // ... other properties
}
```

### Security Configuration
```bicep
// Key Vault with proper access policies
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: resourceNames.keyVault
  location: location
  tags: commonTags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenant().tenantId
    enableRbacAuthorization: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 90
    enablePurgeProtection: true
    networkAcls: {
      defaultAction: 'Deny'
      bypass: 'AzureServices'
    }
  }
}

// Managed Identity for App Service
resource appServiceIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-skilling-${environmentName}-${resourceToken}'
  location: location
  tags: commonTags
}
```

### App Service Configuration
```bicep
resource appService 'Microsoft.Web/sites@2023-01-01' = {
  name: resourceNames.appService
  location: location
  tags: commonTags
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${appServiceIdentity.id}': {}
    }
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      netFrameworkVersion: 'v9.0'
      use32BitWorkerProcess: false
      alwaysOn: true
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      healthCheckPath: '/health'
    }
  }
}
```

## Azure Developer CLI Integration

### Required Structure
- **main.bicep** - primary template in `infra/` folder
- **main.parameters.json** - parameter file with environment-specific values
- **azure.yaml** - project configuration file in root

```bicep
// Ensure azd-env-name tag on resource group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2021-04-01' existing = {
  name: resourceGroup().name
}

// Apply tags to resource group (when using subscription scope)
resource resourceGroupTags 'Microsoft.Resources/tags@2021-04-01' = {
  name: 'default'
  scope: resourceGroup
  properties: {
    tags: {
      'azd-env-name': environmentName
    }
  }
}
```

### Output Configuration
- **Provide necessary outputs** for application configuration
- **Use descriptive output names** matching application settings
- **Include connection strings** and endpoints

```bicep
@description('The URL of the deployed application')
output applicationUrl string = 'https://${appService.properties.defaultHostName}'

@description('Key Vault URI for application secrets')
output keyVaultUri string = keyVault.properties.vaultUri

@description('Managed Identity Client ID')
output managedIdentityClientId string = appServiceIdentity.properties.clientId
```

## Environment Management

### Parameter Files
```json
// main.parameters.json
{
  "$schema": "https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#",
  "contentVersion": "1.0.0.0",
  "parameters": {
    "environmentName": {
      "value": "${AZURE_ENV_NAME}"
    },
    "location": {
      "value": "${AZURE_LOCATION}"
    }
  }
}
```

### Resource Naming
```bicep
// Use environment-aware naming
var environmentConfig = {
  dev: {
    appServicePlanSku: 'B1'
    enableApplicationInsights: true
  }
  staging: {
    appServicePlanSku: 'S1'
    enableApplicationInsights: true
  }
  prod: {
    appServicePlanSku: 'P1v3'
    enableApplicationInsights: true
  }
}

var config = environmentConfig[environmentName]
```

## Validation and Testing

### Deployment Validation
- **Use `--what-if` flag** to preview changes
- **Validate templates** before deployment
- **Test in development environment** first
- **Use incremental deployment mode** for safety

```bash
# Validate template
az deployment group validate \
  --resource-group rg-skilling-dev \
  --template-file infra/main.bicep \
  --parameters @infra/main.parameters.json

# Preview changes
az deployment group what-if \
  --resource-group rg-skilling-dev \
  --template-file infra/main.bicep \
  --parameters @infra/main.parameters.json
```

### Error Handling
- **Include proper dependencies** between resources
- **Use conditional deployment** when appropriate
- **Handle resource naming conflicts** gracefully
- **Provide meaningful error messages** in validation

```bicep
// Conditional resource creation
resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = if (enableApplicationInsights) {
  name: 'ai-skilling-${resourceToken}'
  location: location
  tags: commonTags
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}
```

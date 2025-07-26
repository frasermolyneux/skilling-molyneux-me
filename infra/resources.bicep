@description('The primary location for the resources')
param location string

@description('The name of the environment')
param environmentName string

@description('The name of the App Service Plan')
param appServicePlanName string

@description('The name of the Web App')
param webAppName string

@description('Custom tags to apply to all resources')
param tags object

@description('The Azure AD tenant ID for authentication')
param tenantId string = tenant().tenantId

@description('Comma-separated list of admin user emails')
param adminEmails string = ''

var managedIdentityName = 'id-skilling-${split(webAppName, '-')[2]}-${split(webAppName, '-')[3]}'
var keyVaultName = 'kv-skilling-${environmentName}-${substring(uniqueString(resourceGroup().id), 0, 6)}'

resource managedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: managedIdentityName
  location: location
  tags: tags
}

// Key Vault for storing application secrets
resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: tenantId
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    enableRbacAuthorization: true
  }
}

// Role assignment for managed identity to read Key Vault secrets
resource keyVaultSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, managedIdentity.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId(
      'Microsoft.Authorization/roleDefinitions',
      '4633458b-17de-408a-b874-0445c86b69e6'
    ) // Key Vault Secrets User
    principalId: managedIdentity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

// NOTE: App Registration secrets will be created by GitHub Actions script
// This allows the infrastructure to be deployed first, then the app registration
// secrets are added to the Key Vault in a separate step

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: {
    name: 'B1'
    tier: 'Basic'
    size: 'B1'
    family: 'B'
    capacity: 1
  }
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux plans
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  tags: union(tags, {
    'azd-service-name': 'web'
  })
  kind: 'app,linux'
  properties: {
    serverFarmId: appServicePlan.id
    reserved: true // Required for Linux apps
    httpsOnly: true
    clientAffinityEnabled: false // Improve performance for stateless apps

    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|9.0'
      alwaysOn: true

      minTlsVersion: '1.2'
      scmMinTlsVersion: '1.2'
      ftpsState: 'Disabled' // Disable FTP for security

      healthCheckPath: '/health'

      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environmentName
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
        {
          name: 'KeyVaultUri'
          value: keyVault.properties.vaultUri
        }
        {
          name: 'AzureAd__Instance'
          value: environment().authentication.loginEndpoint
        }
        {
          name: 'AzureAd__Domain'
          value: '${tenantId}.onmicrosoft.com'
        }
        {
          name: 'AzureAd__TenantId'
          value: tenantId
        }
        {
          name: 'AzureAd__CallbackPath'
          value: '/signin-oidc'
        }
        {
          name: 'AdminEmails'
          value: adminEmails
        }
      ]
    }
  }

  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${managedIdentity.id}': {}
    }
  }
}

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output appServicePlanName string = appServicePlan.name
output webAppIdentityPrincipalId string = managedIdentity.properties.principalId
output managedIdentityId string = managedIdentity.id
output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri

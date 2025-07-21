targetScope = 'subscription'

// Parameters
@description('The name of the environment (e.g., dev, staging, prod)')
param environmentName string = 'dev'

@description('The primary location for the resources')
param location string = 'uksouth'

@description('The resource token to make resource names unique')
param resourceToken string = uniqueString(subscription().id, location, environmentName)

@description('The timestamp for the deployment')
param timestamp string = utcNow()

// Variables
var resourceGroupName = 'rg-skilling-${environmentName}'
var appServicePlanName = 'asp-skilling-${environmentName}-${resourceToken}'
var webAppName = 'app-skilling-${environmentName}-${resourceToken}'

// Define standard tags for all environments
var standardTags = {
  Environment: environmentName
  Workload: 'skilling-molyneux-me'
  DeployedBy: 'AZD-Bicep'
  Git: 'https://github.com/frasermolyneux/skilling-molyneux-me'
  'azd-env-name': environmentName
  CreatedDate: timestamp
}

// Resource Group
resource resourceGroup 'Microsoft.Resources/resourceGroups@2024-11-01' = {
  name: resourceGroupName
  location: location
  tags: standardTags
}

// Deploy the main infrastructure
module mainResources 'resources.bicep' = {
  name: 'main-resources'
  scope: resourceGroup
  params: {
    location: location
    environmentName: environmentName
    appServicePlanName: appServicePlanName
    webAppName: webAppName
    tags: standardTags
  }
}

// Outputs
output RESOURCE_GROUP_ID string = resourceGroup.id
output resourceGroupName string = resourceGroup.name
output webAppName string = mainResources.outputs.webAppName
output webAppUrl string = mainResources.outputs.webAppUrl
output appServicePlanName string = mainResources.outputs.appServicePlanName

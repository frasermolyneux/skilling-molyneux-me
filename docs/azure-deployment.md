# Azure Developer CLI Deployment Guide

This guide explains how to deploy the MX.Skilling application using Azure Developer CLI (azd).

## Prerequisites

1. Install [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/install-azd)
2. Install [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli)
3. Have an Azure subscription

## First Time Setup

### 1. Login to Azure
```bash
azd auth login
```

### 2. Initialize the environment
```bash
azd init
```

### 3. Set environment variables
```bash
# Set the environment name (dev, staging, prod)
azd env set AZURE_ENV_NAME dev

# Set the Azure location (REQUIRED for subscription-scoped deployments)
azd env set AZURE_LOCATION uksouth
```

**Important:** The `AZURE_LOCATION` environment variable is required when using subscription-scoped Bicep templates (which use `targetScope = 'subscription'`).

## Deployment Commands

### Deploy Everything (Infrastructure + Application)
```bash
azd up
```

### Deploy Only Infrastructure
```bash
azd provision
```

### Deploy Only Application Code
```bash
azd deploy
```

### View Deployment Status
```bash
azd show
```

### View Application Logs
```bash
azd logs
```

## Environment Management

### List Environments
```bash
azd env list
```

### Switch Environment
```bash
azd env select <environment-name>
```

### View Environment Variables
```bash
azd env get-values
```

## Cleanup

### Delete Resources
```bash
azd down
```

## Troubleshooting

### Common Issues

#### Location Property Missing Error
If you encounter:
```
InvalidDeployment: The 'location' property must be specified for 'deployment-name'
```

This means the `AZURE_LOCATION` environment variable is not set. Fix with:
```bash
azd env set AZURE_LOCATION uksouth
azd env get-values  # Verify it's set
```

#### Package Output Not Found Error
If you encounter:
```
ERROR: package output 'C:\Users\...\AppData\Local\Temp\azd...\publish' does not exist
```

This means the `dist` property in `azure.yaml` is incorrectly configured. Ensure your `azure.yaml` looks like this:
```yaml
services:
  web:
    project: ./src/MX.Skilling.Web
    host: appservice
    language: dotnet
    # Note: No 'dist' property needed for .NET projects
```

#### Test Packaging Separately
```bash
azd package --all
```

### General Debugging

### View Deployment Details
```bash
azd show --output json
```

### Debug Deployment Issues
```bash
azd provision --debug
```

### Check Resource Status
Visit the Azure Portal and navigate to the resource group created by azd (typically named `rg-<environment-name>`).

## Configuration Files

- `azure.yaml` - Main configuration file for azd
- `infra/main.bicep` - Infrastructure as Code template
- `infra/main.parameters.*.json` - Environment-specific parameters
- `.azure/config.json` - Default settings (optional)

## Notes

- The application includes a `/health` endpoint for Azure App Service health checks
- Resources are tagged with `azd-env-name` for identification
- The deployment uses a user-assigned managed identity for enhanced security
- The App Service plan defaults to Free tier (F1) for cost optimization

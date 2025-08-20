---
applyTo: '**/.github/workflows/*.yml, **/.github/workflows/*.yaml'
---

# GitHub Actions Guidelines

## Core Principles

### Workflow Organization
- **One workflow per purpose** - separate CI, CD, and maintenance workflows
- **Use descriptive names** - workflows should clearly indicate their purpose
- **Pin action versions** - use specific SHA or version tags, not `@main`
- **Use semantic versioning** for workflow triggers and releases

### Security Best Practices
- **Use GITHUB_TOKEN** when possible instead of PATs
- **Store secrets in GitHub Secrets** - never hardcode sensitive values
- **Use environment protection rules** for production deployments
- **Limit permissions** using `permissions` blocks
- **Use OIDC for cloud providers** when available

```yaml
permissions:
  contents: read
  id-token: write # For OIDC
```

## Workflow Structure

### Standard CI/CD Pipeline
```yaml
name: 'Build and Deploy'

on:
  push:
    branches: [main]
  pull_request:
    branches: [main]

env:
  DOTNET_VERSION: '9.0.x'

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v5

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}

      - name: Restore dependencies
        run: dotnet restore src/MX.Skilling.sln

      - name: Build
        run: dotnet build src/MX.Skilling.sln --no-restore --configuration Release

      - name: Test
        run: dotnet test src/MX.Skilling.sln --no-build --configuration Release --filter "Category=Unit|Category=Integration"
```

### Environment-Specific Deployments
- **Use environments** for staging/production distinction
- **Implement approval gates** for production
- **Use conditional deployments** based on branch/tag

```yaml
deploy-production:
  if: github.ref == 'refs/heads/main'
  needs: [build, test]
  runs-on: ubuntu-latest
  environment: production
  steps:
    # deployment steps
```

## Best Practices

### Performance Optimization
- **Cache dependencies** appropriately
- **Use matrix builds** for multiple configurations
- **Parallelize independent jobs**
- **Use `fail-fast: false`** when testing multiple configurations

```yaml
strategy:
  fail-fast: false
  matrix:
    os: [ubuntu-latest, windows-latest]
    dotnet-version: ['8.0.x', '9.0.x']
```

### Error Handling & Reliability
- **Set appropriate timeouts** to prevent hanging jobs
- **Use `continue-on-error`** judiciously
- **Implement retry logic** for flaky operations
- **Upload artifacts** for debugging failures

```yaml
- name: Upload test results
  uses: actions/upload-artifact@v4
  if: failure()
  with:
    name: test-results
    path: TestResults/
```

### Dependency Management
- **Pin major versions** of actions (e.g., `@v4`)
- **Review action updates** through Dependabot
- **Use official actions** when possible
- **Verify third-party actions** before use

## Specific Patterns

### .NET Projects
```yaml
- name: Setup .NET
  uses: actions/setup-dotnet@v4
  with:
    dotnet-version: '9.0.x'

- name: Restore dependencies
  run: dotnet restore src/MX.Skilling.sln

- name: Build
  run: dotnet build src/MX.Skilling.sln --no-restore --configuration Release

- name: Test with Coverage
  run: dotnet test src/MX.Skilling.sln --no-build --configuration Release --collect:"XPlat Code Coverage" --filter "Category=Unit|Category=Integration"

- name: Format Check
  run: dotnet format src/MX.Skilling.sln --verify-no-changes
```

### Azure Deployment
```yaml
- name: Azure Login (OIDC)
  uses: azure/login@v2
  with:
    client-id: ${{ secrets.AZURE_CLIENT_ID }}
    tenant-id: ${{ secrets.AZURE_TENANT_ID }}
    subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

- name: Deploy Infrastructure
  run: |
    az deployment group create \
      --resource-group ${{ vars.AZURE_RESOURCE_GROUP }} \
      --template-file infra/main.bicep \
      --parameters @infra/main.parameters.json
```

### Conditional Logic
- **Use `if` conditions** to control job/step execution
- **Check for specific file changes** using path filters
- **Implement branch-based logic** appropriately

```yaml
- name: Deploy to staging
  if: github.event_name == 'pull_request'
  # staging deployment steps

- name: Deploy to production
  if: github.ref == 'refs/heads/main' && github.event_name == 'push'
  # production deployment steps
```

## Workflow Examples

### Pull Request Validation
```yaml
name: 'PR Validation'

on:
  pull_request:
    branches: [main]

jobs:
  validate:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v5
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'
      - name: Validate
        run: |
          dotnet restore src/MX.Skilling.sln
          dotnet build src/MX.Skilling.sln --no-restore
          dotnet test src/MX.Skilling.sln --no-build --filter "Category=Unit|Category=Integration"
          dotnet format src/MX.Skilling.sln --verify-no-changes
```

### Automated Releases
```yaml
name: 'Release'

on:
  push:
    tags:
      - 'v*'

jobs:
  release:
    runs-on: ubuntu-latest
    permissions:
      contents: write
    steps:
      - uses: actions/checkout@v5
      - name: Create Release
        uses: actions/create-release@v1
        with:
          tag_name: ${{ github.ref }}
          release_name: Release ${{ github.ref }}
```

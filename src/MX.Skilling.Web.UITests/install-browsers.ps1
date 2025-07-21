# Playwright Installation Script
# This script installs Playwright browsers needed for UI testing

Write-Host "Installing Playwright browsers..." -ForegroundColor Green

# Install Playwright browsers
dotnet tool install --global Microsoft.Playwright.CLI --version 1.52.0
playwright install chromium firefox webkit

Write-Host "Playwright browsers installed successfully!" -ForegroundColor Green
Write-Host "You can now run UI tests using:" -ForegroundColor Yellow
Write-Host "  dotnet test src/MX.Skilling.Web.UITests/" -ForegroundColor Yellow

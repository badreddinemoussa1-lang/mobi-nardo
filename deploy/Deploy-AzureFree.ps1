param(
    [string] $Location = "westeurope"
)

$ErrorActionPreference = "Stop"

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw "Azure CLI is required. Install it from https://learn.microsoft.com/cli/azure/install-azure-cli, open a new PowerShell window, then run az login."
}

az account show --output none
if ($LASTEXITCODE -ne 0) {
    throw "Sign in first with: az login"
}

$suffix = [Guid]::NewGuid().ToString("N").Substring(0, 8)
$appName = "mobi-nardo-$suffix"
$resourceGroup = "rg-mobi-nardo-$suffix"
$planName = "plan-mobi-nardo-$suffix"
$projectRoot = Split-Path -Parent $PSScriptRoot
$publishDirectory = Join-Path $env:TEMP "mobi-nardo-publish-$suffix"
$zipPath = Join-Path $env:TEMP "mobi-nardo-deploy-$suffix.zip"
$bootstrapKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))

Write-Host "Publishing MOBI NARDO..."
dotnet publish (Join-Path $projectRoot "MotoMobiNardo.csproj") --configuration Release --output $publishDirectory
if ($LASTEXITCODE -ne 0) { throw "dotnet publish failed." }

Compress-Archive -Path (Join-Path $publishDirectory "*") -DestinationPath $zipPath -Force

Write-Host "Creating an Azure App Service on the Free F1 plan in $Location..."
az group create --name $resourceGroup --location $Location --output none
if ($LASTEXITCODE -ne 0) { throw "Could not create the Azure resource group." }

az appservice plan create --name $planName --resource-group $resourceGroup --location $Location --sku F1 --is-linux --output none
if ($LASTEXITCODE -ne 0) { throw "Could not create the free Azure App Service plan. No paid plan was selected." }

az webapp create --name $appName --resource-group $resourceGroup --plan $planName --runtime "DOTNETCORE:10.0" --output none
if ($LASTEXITCODE -ne 0) { throw "Could not create the .NET 10 web app." }

az webapp config appsettings set --name $appName --resource-group $resourceGroup --settings `
    ASPNETCORE_ENVIRONMENT=Production `
    ASPNETCORE_URLS=http://0.0.0.0:8080 `
    MOBI_NARDO_DATA_PATH=/home/data `
    MOBI_NARDO_BOOTSTRAP_KEY=$bootstrapKey `
    WEBSITES_ENABLE_APP_SERVICE_STORAGE=true `
    --output none
if ($LASTEXITCODE -ne 0) { throw "Could not configure persistent app storage and startup settings." }

Write-Host "Uploading the published site..."
az webapp deploy --name $appName --resource-group $resourceGroup --src-path $zipPath --type zip --timeout 600000 --output none
if ($LASTEXITCODE -ne 0) { throw "Deployment failed. The Azure resource group is $resourceGroup." }

$siteUrl = "https://$appName.azurewebsites.net"
$setupUrl = "$siteUrl/Account/Setup?key=$([Uri]::EscapeDataString($bootstrapKey))"
$ready = $false
for ($attempt = 0; $attempt -lt 36; $attempt++) {
    try {
        $response = Invoke-WebRequest -Uri $siteUrl -TimeoutSec 15 -UseBasicParsing
        if ($response.StatusCode -eq 200) { $ready = $true; break }
    }
    catch { Start-Sleep -Seconds 10 }
}

Write-Host ""
Write-Host "MOBI NARDO URL: $siteUrl"
if ($ready) { Write-Host "Public site responded with HTTP 200." }
else { Write-Warning "Azure deployment completed, but the first HTTP check did not return 200 yet. Open the site and check Azure deployment logs." }
Write-Host "ONE-TIME ADMIN SETUP: $setupUrl"
Write-Host "After creating the admin account, remove MOBI_NARDO_BOOTSTRAP_KEY from this App Service's Configuration settings."
Write-Host "Resource group: $resourceGroup (Free F1 plan; no paid compute tier was created)."
Write-Host "Existing local customer, sales, and admin database records were not uploaded. Public sample stock is seeded from appsettings.json."

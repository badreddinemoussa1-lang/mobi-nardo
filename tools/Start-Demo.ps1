$ErrorActionPreference = "Stop"
$projectRoot = Split-Path -Parent $PSScriptRoot
$projectFile = Join-Path $projectRoot "MotoMobiNardo.csproj"
$demoDataPath = Join-Path $projectRoot "App_Data\mobi-nardo-demo"

# The application rejects demo mode if it is pointed at the production data folder.
$env:MOBI_NARDO_DATA_PATH = $demoDataPath
$env:MOBI_NARDO_DEMO_MODE = "true"
try {
    dotnet run --project $projectFile
    if ($LASTEXITCODE -ne 0) { throw "The demo server exited with code $LASTEXITCODE." }
}
finally {
    Remove-Item Env:MOBI_NARDO_DATA_PATH -ErrorAction SilentlyContinue
    Remove-Item Env:MOBI_NARDO_DEMO_MODE -ErrorAction SilentlyContinue
}

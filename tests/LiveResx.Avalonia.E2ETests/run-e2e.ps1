# =================================================================================
# LiveResx.Avalonia e2e test runner script (PowerShell)
# =================================================================================
#
# Description:
#   This script runs e2e tests. It relies on the locally published NuGet package.
#   It can be run from any folder.
#
# Usage:
#   .\run-e2e.ps1
#
# =================================================================================

$ErrorActionPreference = 'Stop'

$Configuration = 'Release'
$NugetConfigFile = 'nuget.e2e.config'
$LocalPackageCacheFolder = 'packages'
$TimingOutputFolder = 'timings'

function Write-Banner([string[]]$Lines, [ConsoleColor]$Color = 'Magenta') {
    Write-Host '========================================================' -ForegroundColor $Color
    foreach ($line in $Lines) { Write-Host $line -ForegroundColor $Color }
    Write-Host '========================================================' -ForegroundColor $Color
}

# Exit immediately if a native command returns a non-zero status.
function Invoke-Native([scriptblock]$Command) {
    & $Command
    if ($LASTEXITCODE -ne 0) { throw "Command failed with exit code ${LASTEXITCODE}: $Command" }
}

function Remove-Folder([string]$Path) {
    if (Test-Path $Path) { Remove-Item -Recurse -Force $Path }
}

Push-Location $PSScriptRoot
try {
    Write-Banner 'Restoring dependencies'
    Remove-Folder $LocalPackageCacheFolder
    Invoke-Native { dotnet restore LiveResx.Avalonia.E2ETests.csproj --packages $LocalPackageCacheFolder --configfile $NugetConfigFile }
    Write-Host

    Write-Banner 'Building E2E test project', "Configuration: $Configuration"
    Invoke-Native { dotnet clean LiveResx.Avalonia.E2ETests.csproj }
    Invoke-Native { dotnet build LiveResx.Avalonia.E2ETests.csproj -c $Configuration --packages $LocalPackageCacheFolder --no-restore }
    Write-Host

    Write-Banner 'Running E2E tests', "Configuration: $Configuration"
    Remove-Folder $TimingOutputFolder
    # The package folder must still exist here: without it the test SDK's build
    # props are not imported and dotnet test silently runs nothing.
    Invoke-Native { dotnet test LiveResx.Avalonia.E2ETests.csproj -c $Configuration --no-restore }
    Write-Host

    Write-Banner 'E2E tests run successfully.' -Color Green
} finally {
    Remove-Folder $TimingOutputFolder
    Remove-Folder $LocalPackageCacheFolder
    Pop-Location
}

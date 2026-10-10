# =================================================================================
# LiveResx.Avalonia local publish script (PowerShell)
# =================================================================================
#
# Description:
#   This script builds the solution, packs it into a NuGet package and stores locally.
#   It can be run from any folder.
#
# Usage:
#   .\local-publish.ps1
#
# =================================================================================

$ErrorActionPreference = 'Stop'

$Configuration = 'Release'
$PublishFolder = 'artifacts'
$NugetConfigFile = 'nuget.e2e.config'

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

Push-Location $PSScriptRoot
try {
    Write-Banner 'Building LiveResx.Avalonia', "Configuration: $Configuration"
    Push-Location ../../src
    try {
        Invoke-Native { dotnet build LiveResx.Avalonia.slnx -c $Configuration }
    } finally {
        Pop-Location
    }
    Write-Host

    Write-Banner 'Publishing NuGet package to local folder', "Configuration: $Configuration"
    Push-Location ../..
    try {
        if (Test-Path $PublishFolder) { Remove-Item -Recurse -Force $PublishFolder }
        Invoke-Native { dotnet pack -c $Configuration -o $PublishFolder src/LiveResx.Avalonia.slnx --no-build }
    } finally {
        Pop-Location
    }
    Write-Host

    Write-Banner 'Published successfully.' -Color Green
    Write-Host

    Write-Banner 'Creating local NuGet config'
    if (Test-Path $NugetConfigFile) { Remove-Item -Force $NugetConfigFile }
    Invoke-Native { dotnet new nugetconfig }
    Move-Item nuget.config $NugetConfigFile
    Invoke-Native { dotnet nuget add source "../../$PublishFolder" -n local-packages --configfile $NugetConfigFile }
    Write-Host

    Write-Banner 'Created local NuGet config successfully.' -Color Green
} finally {
    Pop-Location
}

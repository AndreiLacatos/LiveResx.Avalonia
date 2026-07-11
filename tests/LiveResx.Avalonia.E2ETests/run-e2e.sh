#!/bin/bash
# =================================================================================
# LiveResx.Avalonia e2e test runner script (Bash)
# =================================================================================
#
# Description:
#   This script runs e2e tests. It relies on the locally published NuGet package.
#   It detects whether it's being run from the root or the test folder.
#
# Usage:
#   ./run-e2e.sh
#
# =================================================================================

set -e

CONFIGURATION="Release"
NUGET_CONFIG_FILE="nuget.e2e.config"
LOCAL_PACKAGE_CACHE_FOLDER="packages"
TIMING_OUTPUT_FOLDER="timings"

# Detect if running from project root; if so, cd to tests/LiveResx.Avalonia.E2ETests
if [ ! -f "LiveResx.Avalonia.E2ETests.csproj" ]; then
  if [ -f "tests/LiveResx.Avalonia.E2ETests/LiveResx.Avalonia.E2ETests.csproj" ]; then
    cd tests/LiveResx.Avalonia.E2ETests
  else
    echo "ERROR: Could not locate LiveResx.Avalonia.E2ETests.csproj."
    exit 1
  fi
fi

COLOR_GREEN='\033[92m'
COLOR_RESET='\033[0m'
COLOR_PURPLE='\033[35m'

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Restoring dependencies"
echo "========================================================"
printf "${COLOR_RESET}"

if [ -d "${LOCAL_PACKAGE_CACHE_FOLDER}" ]; then
    rm -rf "${LOCAL_PACKAGE_CACHE_FOLDER}"
fi

dotnet restore LiveResx.Avalonia.E2ETests.csproj --packages "${LOCAL_PACKAGE_CACHE_FOLDER}" --configfile "${NUGET_CONFIG_FILE}"
echo

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Building E2E test project"
echo "Configuration: ${CONFIGURATION}"
echo "========================================================"
printf "${COLOR_RESET}"

dotnet clean LiveResx.Avalonia.E2ETests.csproj
dotnet build LiveResx.Avalonia.E2ETests.csproj -c "${CONFIGURATION}" --packages "${LOCAL_PACKAGE_CACHE_FOLDER}" --no-restore
echo

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Running E2E tests"
echo "Configuration: ${CONFIGURATION}"
echo "========================================================"
printf "${COLOR_RESET}"

if [ -d "${TIMING_OUTPUT_FOLDER}" ]; then
    rm -rf "${TIMING_OUTPUT_FOLDER}"
fi
if [ -d "${LOCAL_PACKAGE_CACHE_FOLDER}" ]; then
    rm -rf "${LOCAL_PACKAGE_CACHE_FOLDER}"
fi

dotnet test LiveResx.Avalonia.E2ETests.csproj -c "${CONFIGURATION}" --no-restore

if [ -d "${TIMING_OUTPUT_FOLDER}" ]; then
    rm -rf "${TIMING_OUTPUT_FOLDER}"
fi
echo

printf "${COLOR_GREEN}"
echo "========================================================"
echo "E2E tests run successfully."
echo "========================================================"
printf "${COLOR_RESET}"

exit 0
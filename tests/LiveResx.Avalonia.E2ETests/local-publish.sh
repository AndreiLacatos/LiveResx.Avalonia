#!/bin/bash
# =================================================================================
# LiveResx.Avalonia local publish script (Bash)
# =================================================================================
#
# Description:
#   This script builds the solution, packs it into a NuGet package and stores locally.
#
# Usage:
#   ./local-publish.sh
#
# =================================================================================

# Exit immediately if a command returns a non-zero status.
set -e

CONFIGURATION="Release"
PUBLISH_FOLDER="artifacts"
NUGET_CONFIG_FILE="nuget.e2e.config"

COLOR_GREEN='\033[92m'
COLOR_RESET='\033[0m'
COLOR_PURPLE='\033[35m'

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Building LiveResx.Avalonia"
echo "Configuration: ${CONFIGURATION}"
echo "========================================================"
printf "${COLOR_RESET}"
pushd ../../src > /dev/null
dotnet build LiveResx.Avalonia.slnx -c "${CONFIGURATION}"
popd > /dev/null
echo

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Publishing NuGet package to local folder"
echo "Configuration: ${CONFIGURATION}"
echo "========================================================"
printf "${COLOR_RESET}"
pushd ../../src/.. > /dev/null
if [ -d "${PUBLISH_FOLDER}" ]; then
    rm -rf "${PUBLISH_FOLDER}"
fi
dotnet pack -c "${CONFIGURATION}" -o "${PUBLISH_FOLDER}" src/LiveResx.Avalonia.slnx --no-build
popd > /dev/null
echo

printf "${COLOR_GREEN}"
echo "========================================================"
echo "Published successfully."
echo "========================================================"
printf "${COLOR_RESET}"
echo

printf "${COLOR_PURPLE}"
echo "========================================================"
echo "Creating local NuGet config"
echo "========================================================"
printf "${COLOR_RESET}"

if [ -f "${NUGET_CONFIG_FILE}" ]; then
    rm "${NUGET_CONFIG_FILE}"
fi

dotnet new nugetconfig
mv nuget.config "${NUGET_CONFIG_FILE}"
dotnet nuget add source "../../${PUBLISH_FOLDER}" -n local-packages --configfile "${NUGET_CONFIG_FILE}"
echo

printf "${COLOR_GREEN}"
echo "========================================================"
echo "Created local NuGet config successfully."
echo "========================================================"
printf "${COLOR_RESET}"

exit 0
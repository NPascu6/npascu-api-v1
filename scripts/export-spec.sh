#!/usr/bin/env bash
set -euo pipefail

# Build the API project
DOTNET_ROOT=${DOTNET_ROOT:-/usr/share/dotnet}
export PATH="$DOTNET_ROOT:$PATH"

# Restore local tools
if [ -f ".config/dotnet-tools.json" ]; then
  dotnet tool restore >/dev/null
fi

# Build the API project
if [ ! -f "src/Api/bin/Release/net8.0/Api.dll" ]; then
  dotnet build src/Api/Api.csproj -c Release >/dev/null
fi

# Generate the OpenAPI specification to swagger.json
OUTPUT_FILE=${1:-swagger.json}
dotnet tool run swagger tofile --output "$OUTPUT_FILE" src/Api/bin/Release/net8.0/Api.dll v1

echo "OpenAPI specification written to $OUTPUT_FILE"

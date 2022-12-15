#!/bin/bash
echo "Lösche alte Builds..."
find . -type d -name ".vs" -exec rm -rf {} \;
find . -type d -name ".vscode" -exec rm -rf {} \;
find . -type d -name "bin" -exec rm -rf {} \;
find . -type d -name "obj" -exec rm -rf {} \;

echo "Starte Server"
cd StoreManager.Webapp
export STORE_ADMIN=EinAdminPasswort
export ASPNETCORE_ENVIRONMENT=Development
dotnet watch run --no-launch-profile

#!/usr/bin/env bash

dotnet build -c Release

dotnet pack ./GREngine/GREngine.csproj -p:NuspecFile=GREngine.nuspec --output ./Build

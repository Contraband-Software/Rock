dotnet build -c Release -o Build\artefacts

dotnet pack .\GREngine\GREngine.csproj -p:NuspecFile=GREngine.nuspec --output .\Build

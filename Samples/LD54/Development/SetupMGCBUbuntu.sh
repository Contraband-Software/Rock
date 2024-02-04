sudo wget -qO- https://raw.githubusercontent.com/MonoGame/MonoGame/master/Tools/MonoGame.Effect.Compiler/mgfxc_wine_setup.sh | bash
cd ~/.winemonogame/drive_c/windows/system32

DOTNET_URL="https://download.visualstudio.microsoft.com/download/pr/44d08222-aaa9-4d35-b24b-d0db03432ab7/52a4eb5922afd19e8e0d03e0dbbb41a0/dotnet-sdk-6.0.302-win-x64.zip"
curl "https://download.visualstudio.microsoft.com/download/pr/44d08222-aaa9-4d35-b24b-d0db03432ab7/52a4eb5922afd19e8e0d03e0dbbb41a0/dotnet-sdk-6.0.302-win-x64.zip" --output "dotnet-sdk.zip"

7z x "dotnet-sdk.zip" -y

rm dotnet-sdk.zip

export MGFXC_WINE_PATH="$HOME/.winemonogame/"

echo "Reboot now"

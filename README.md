# **GREngine** :: GREEN ROCK ENGINE

Contraband Software's very own custom game engine, built on top of the reliable Monogame framework. Simple and ready for game jams!

![Green glowing rock outline over black background](./Documentation/Images/rockIcon.png) 

## Prerequisites

This is a .NET Core 6.0 MonoGame Game Library, so you will need both of those things installed. Also Git.

 - [.NET Core 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
 - [MonoGame / XNA](https://monogame.net/articles/getting_started/index.html)

## Usage

You will need to create a MonoGame Desktop GL CSharp project for your game.

 - Clone the repo
 - Run `BUILD_NUPKG.sh` (`BUILD_NUPKG.bat` if you use Windows) to generate a local nuget package + feed
   - You may need to run `chmod +x ./BUILD_NUPKG.sh` on Linux to allow it to run
   - `./Build/` Will contain a `.nupkg` file you'd want to copy and store in your project's external libs folder
 - From your game project, add that local nuget package as a reference
   `dotnet add package software.contraband.GREngine -s "<directory containing the .nupkg file>"

To get started immediately, create your game project in a sister folder to the repo

 - `dotnet add package software.contraband.GREngine -s "../GreenRockEngine/Build/"`
 - Start game dev!

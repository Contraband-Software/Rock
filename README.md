#### using **GREngine**;
# GREEN ROCK ENGINE - Active Development

Contraband Software's very own custom game engine, built on top of the reliable MonoGame framework. Simple and ready for game jams!

![Green glowing rock outline over black background](./Documentation/Images/rockIcon.png) 

## Prerequisites

This is a .NET Core 6.0 MonoGame Game Library, so you will need both of those things installed. Also Git.

 - [.NET Core 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Make sure to install the SDK, which also contains the runtime.
 - [MonoGame / XNA](https://monogame.net/articles/getting_started/index.html), with full shader compiler setup if you're on Linux or MacOS

## Usage

You will need to create a MonoGame Desktop GL CSharp project for your game.

 - Clone the repo
 - Run `BUILD_NUPKG.sh` (`BUILD_NUPKG.bat` if you use Windows) to generate a local nuget package + feed
   - You may need to run `chmod +x ./BUILD_NUPKG.sh` on Linux to allow it to run
   - The generated `./Build/` folder will contain a `.nupkg` file you'd want to copy and store in your project's external libs folder
 - From your game project, add that local nuget package as a reference
   `dotnet add package software.contraband.GREngine -s "<directory containing the .nupkg file>"

To get started immediately, create your game project in a sister folder to the repo

 - `dotnet add package software.contraband.GREngine -s "../GreenRockEngine/Build/"`
 - Start game dev!

## Documentation

Currently a working progress: the [Scripting API Reference](https://contraband-software.github.io/Rock/inherits.html)

`./GameDemo1` gives a good demonstration of how to build a game with this library. It makes use a large use of shaders, they can be learnt about in Monogame's Custom Effects documentation. The engine also provides a number of preset attributes to all effects, which can be read about in the GREngine.Core.PebbleRenderer package.

 - It is important to note that this demo project depends on the [`D3 Digitalism`](./GameDemo1/Content/D3Digitalism.ttf) and `Arial` (Microsoft) fonts being present on your system.

[License](./LICENSE)

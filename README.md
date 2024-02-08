#### using **GREngine**;
# ----> GREEN ROCK ENGINE - Active Development

Contraband Software's very own custom game engine, built on top of the reliable MonoGame framework. Simple and ready for game jams!

![Green glowing rock outline over black background](./Documentation/Images/rockIcon.png) 

## Prerequisites

This is a .NET Core 6.0 MonoGame Game Library, so you will need both of those things installed. Also Git.

 - [.NET Core 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) Make sure to install the SDK, which also contains the runtime.
 - [MonoGame / XNA](https://monogame.net/articles/getting_started/index.html)

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

## License TLDR - If you want to make something awesome

We have licensed this project under LGPL, here are the broad strokes:

 - You may freely: use, distribute, alter, and incorporate this library into commercial works, provided the following terms are followed:
 - You **cannot** statically link this library with proprietary code, no restrictions exist on open source projects with LGPL-compatible licenses.
   - .NET Core dynamically links libraries by default anyway.
 - You must distribute a copy of this project's `LICENSE` text along with any work that uses this library at all
 - If you make any modifications to this library's code, you will need to distribute those changes under LGPL along with any proprietary work that uses that modified version of the library, along with the library's `LICENSE` text
   - You may still keep any code that you wrote yourself proprietary if you wish

*This is not a legal substitute for the full license terms.*

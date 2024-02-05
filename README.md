# **GREngine** :: GREEN ROCK ENGINE

![Green glowing rock outline over black background](./Documentation/Images/rockIcon.png)

Contraband Software's very own custom game engine, built on top of the reliable Monogame framework. Simple and ready for game jams!

## Usage

 - Clone the repo
 - Create a monogame project in a sister folder to the repo
 - Run `BUILD_NUPKG.sh` to generate a local nuget package + feed
   - `./Build/`
 - From the game project, add that local package as a reference
   - `dotnet add package software.contraband.GREngine -s "../GreenRockEngine/Build/"`
 - Start game dev!

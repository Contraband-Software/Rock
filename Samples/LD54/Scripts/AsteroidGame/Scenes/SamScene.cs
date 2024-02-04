#if SAM

global using LD54.Engine;

namespace LD54.AsteroidGame.Scenes;

using System;
using System.Collections.Generic;
using Engine.Dev;
using Engine.Leviathan;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AsteroidGame.GameObjects;
using Engine.Components;
using AsteroidGame.GameObjects;
using Engine;
using LD54.AsteroidGame.GameObjects;

public class SamScene : Scene
{
    #region PARAMS
    public const float FORCE_LAW = 2.4f;
    public const float GRAVITATIONAL_CONSTANT = 43f;

    public const float BLACK_HOLE_MASS = 100;   // no need to edit, GRAVITATIONAL_CONSTANT is already directly proportional (Satellites are massless)
    public const int SATELLITES = 100;
    public const float SPEED_MULT = 15f;

    public const float MAP_SIZE = 2f;
    #endregion

    private int sunLight = -1;

    private Texture2D testObjectTexture;

    public SamScene(Game appCtx) : base("SamScene", appCtx)
    {
    }

    public void SpawnAccretionDisk(GameObject parent, Vector2 boundsOffset, Vector2 boundsDimensions, Vector2 blackHole)
    {
        Random rnd = new Random();

        for (int i = 0; i < GameScene.SATELLITES; i++)
        {
            Vector2 startPosition = new Vector2(
                rnd.Next((int)boundsOffset.X, (int)boundsDimensions.X),
                rnd.Next((int)boundsOffset.Y, (int)boundsDimensions.Y));

            Vector2 separation = startPosition - blackHole;
            Vector2 perpendicular = separation.PerpendicularClockwise();
            perpendicular.Normalize();

            // PrintLn(perpendicular.ToString());

            GameObject newSat = new SatelliteObject(
                0,
                new Vector3(perpendicular.X, perpendicular.Y, 0.76f) * GameScene.SPEED_MULT * (1 / MathF.Sqrt(separation.Magnitude())),
                testObjectTexture,
                "satelliteObject_" + i,
                this.app
            );

            parent.AddChild(newSat);
            newSat.SetLocalPosition(startPosition);
        }
    }

    public override void OnLoad(GameObject? parentObject)
    {
        Vector2 windowSize = this.app.Services.GetService<ILeviathanEngineService>().getWindowSize();
        PrintLn("Screen resolution: " + windowSize);

        // simple scene-wide illumination
        sunLight = this.app.Services.GetService<ILeviathanEngineService>().AddLight(new Vector2(200, 200), new Vector3(10000000, 10000000, 10000000));

        // sim set up
        NewtonianSystemObject newtonianSystem = new NewtonianSystemObject(
            GameScene.GRAVITATIONAL_CONSTANT,
            "GravitySimulationObject",
            this.app);
        parentObject.AddChild(newtonianSystem);
        newtonianSystem.SetLocalPosition(new Vector2(150, 150));

        testObjectTexture = this.contentManager.Load<Texture2D>("Sprites/circle");

        // player controller

        //DebugPlayer playerd = new(testObjectTexture, "DebugPlayerController", this.app);
        //newtonianSystem.AddChild(playerd);


        // black hole
        Vector2 blackHolePosition = new Vector2(0, 0);
        GameObject blackHole = new BlackHole(
            BLACK_HOLE_MASS,
            testObjectTexture,
            "BlackHole",
            this.app
            );
        newtonianSystem.AddChild(blackHole);
        blackHole.SetLocalPosition(blackHolePosition);

        Texture2D shipTexture = this.contentManager.Load<Texture2D>("Sprites/arrow");
        Texture2D shipTextureBoost = this.contentManager.Load<Texture2D>("Sprites/arrow");
        Spaceship player = new Spaceship(blackHole as BlackHole, shipTexture, shipTextureBoost,"player", app);
        player.SetLocalPosition(new Vector2(-300, 150));
        parentObject.AddChild(player);

        // some testing space junk spawning
        SpawnAccretionDisk(newtonianSystem,
            new Vector2(
                windowSize.X * -MAP_SIZE /2,
                windowSize.Y * -MAP_SIZE /2
            ) + blackHolePosition,
            new Vector2(
                windowSize.X * MAP_SIZE,
                windowSize.Y * MAP_SIZE
            ),
            blackHolePosition
        );
    }

    private bool printed = false;
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!printed)
        {
            printed = true;
            this.app.Services.GetService<ISceneControllerService>().DebugPrintGraph();
        }
    }

    public override void OnUnload()
    {
        this.app.Services.GetService<ILeviathanEngineService>().removeLight(this.sunLight);
    }
}
#endif

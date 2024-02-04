global using static LD54.Engine.Dev.EngineDebug;
namespace LD54;

using System;
using System.Diagnostics;
using AsteroidGame.Scenes;
using Engine;
using Engine.Leviathan;
using Engine.Collision;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using MonoGame.OpenGL;

public class App : Game
{
    private readonly GraphicsDeviceManager graphics;

    private readonly SceneController sc;
    private readonly LeviathanEngine le;
    private readonly CollisionSystem cs;

    public App()
    {
        this.graphics = new GraphicsDeviceManager(this);



        #region VIDEO_MODE_SELECTION
        bool debugVideoMode = false;
        if (debugVideoMode)
        {
            this.graphics.PreferredBackBufferWidth = 1200;
            this.graphics.PreferredBackBufferHeight = 800;
        }
        else
        {
            float largestResolution = 0;
            DisplayMode? preferredMode = null;
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                float size = mode.Width * mode.Height;
                if (size > largestResolution)
                {
                    largestResolution = size;
                    preferredMode = mode;
                }
            }
            this.graphics.PreferredBackBufferWidth = preferredMode.Width;
            this.graphics.PreferredBackBufferHeight = preferredMode.Height;
            this.graphics.ToggleFullScreen();
            // PrintLn(preferredMode.ToString());
        }

        PrintLn(this.GraphicsDevice.Viewport.Width + " " + this.GraphicsDevice.Viewport.Height);
        #endregion

        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;

        le = new LeviathanEngine(this);
        sc = new SceneController(this);
        cs = new CollisionSystem(this);
    }

    protected override void Initialize() {
        this.Components.Add(le);
        this.Services.AddService(typeof(ILeviathanEngineService), le);

        this.Components.Add(sc);
        this.Services.AddService(typeof(ISceneControllerService), sc);

        this.Components.Add(cs);
        this.Services.AddService(typeof(ICollisionSystemService), cs);

        PrintLn("App: Game systems initialized.");

        this.sc.AddScene(new StartScene(6, this));//6
        this.sc.AddScene(new GameScene(this));

        PrintLn("App: Scenes loaded.");

        this.sc.ChangeScene("StartScene");

        PrintLn("App: Game scene started.");

        // ALL BASE FUNCTION CALLS MUST COME LAST !!!! OTHERWISE NONE OF OUR SERVICES GET INITIALIZED
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: GLOBAL LOAD CONTENT, USE THE GLOBAL CONTENT MANAGER CONTAINED IN GAME TO LOAD PERSISTENT CONTENT.
        // The statement below demonstrates the global content manager
        // this.Content.Load<>()
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            this.Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}

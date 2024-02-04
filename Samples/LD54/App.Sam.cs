#if SAM

global using static LD54.Engine.Dev.EngineDebug;

namespace LD54;

using AsteroidGame.Scenes;
using Engine;
using Engine.Leviathan;
using Engine.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;

public class App_Sam : Game
{
    private readonly GraphicsDeviceManager graphics;
    private SceneController sc;
    private LeviathanEngine le;
    private CollisionSystem cs;

    #region PROTOTYPING_SUPPORT_RENDER
    private SpriteBatch spriteBatch;
    Texture2D boxTexture;

    // Vector2 position

    void SetupPrototyping()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        boxTexture = Content.Load<Texture2D>("Sprites/block");
    }

    void DrawPrototyping()
    {
        spriteBatch.Begin();
        spriteBatch.Draw(boxTexture, new Vector2(0, 0), Color.White);
        spriteBatch.End();
    }
    #endregion

    public App_Sam()
    {
        this.graphics = new GraphicsDeviceManager(this);
        // this.graphics.ToggleFullScreen();
        this.graphics.PreferredBackBufferHeight = 1000;
        this.graphics.PreferredBackBufferWidth = 1600;
        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
    }

    protected override void Initialize() {
        le = new LeviathanEngine(this);
        this.Components.Add(le);
        this.Services.AddService(typeof(ILeviathanEngineService), le);

        sc = new SceneController(this);
        this.Components.Add(sc);
        this.Services.AddService(typeof(ISceneControllerService), sc);

        cs = new CollisionSystem(this);
        this.Components.Add(cs);
        this.Services.AddService(typeof(ICollisionSystemService), cs);

        PrintLn("App: Game systems initialized.");

        this.sc.AddScene(new SamScene(this));

        PrintLn("App: Scenes loaded.");

        this.sc.ChangeScene("SamScene");

        PrintLn("App: Game scene started.");

        // THESE base METHODS MUST ALWAYS COME LAST IN THE FUNCTION
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: GLOBAL LOAD CONTENT, USE THE GLOBAL CONTENT MANAGER CONTAINED IN GAME TO LOAD PERSISTENT CONTENT.
        // The statement below demonstrates the global content manager
        // this.Content.Load<>()

        SetupPrototyping();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            this.Exit();
        }

        // TODO: UPDATE OUR SERVICES HERE

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);



        base.Draw(gameTime);
    }
}
#endif

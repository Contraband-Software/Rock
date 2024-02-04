global using static LD54.Engine.Dev.EngineDebug;
namespace LD54;

using AsteroidGame.Scenes;
using Engine;
using Engine.Leviathan;
using Engine.Collision;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class App_Julius : Game
{
    private readonly GraphicsDeviceManager graphics;
    private SceneController sc;
    private LeviathanEngine le;
    private CollisionSystem cs;

    public App_Julius()
    {
        this.graphics = new GraphicsDeviceManager(this);
        this.Content.RootDirectory = "Content";
        this.IsMouseVisible = true;
    }

    protected override void Initialize()
    {

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

        this.sc.AddScene(new JuliusScene(this));

        PrintLn("App: Scenes loaded.");

        this.sc.ChangeScene("JuliusScene");

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

        // TODO: UPDATE OUR SERVICES HERE

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);

        base.Draw(gameTime);
    }
}

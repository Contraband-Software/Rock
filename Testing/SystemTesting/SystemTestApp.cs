using GREngine.Core.PebbleRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Testing;

using System.Diagnostics;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using SystemTesting;

public class SystemTestApp : Game
{
    private GraphicsDeviceManager graphics;

    private readonly CollisionSystem cs;
    private readonly PebbleRenderer re;
    private readonly SceneManager sceneManager;

    public SystemTestApp()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        re = new PebbleRenderer(this, graphics, 1000, 600, 1f, 0.5f);
        cs = new CollisionSystem(this);
        sceneManager = new SceneManager(this);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.Components.Add(sceneManager);
        this.Services.AddService(typeof(ISceneControllerService), sceneManager);

        this.Components.Add(cs);
        this.Services.AddService(typeof(ICollisionSystem), cs);

        this.Components.Add(re);
        this.Services.AddService(typeof(IPebbleRendererService), re);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        re.LoadShaders();

        Scene myScene = new MyScene();
        this.sceneManager.AddScene(myScene);

        this.sceneManager.ChangeScene("MyScene");
    }

    private bool done = false;
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!done)
        {
            sceneManager.DebugPrintGraph();
            done = true;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        //GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        //base.Draw(gameTime);

        re.Draw(gameTime);
    }
}

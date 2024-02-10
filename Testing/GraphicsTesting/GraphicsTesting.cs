using GREngine.Core.PebbleRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Testing;

using GREngine.Core.System;
using System;
using System.Diagnostics;
using SystemTesting;

public class GraphicsTesting : Game
{
    private GraphicsDeviceManager graphics;
    //private SpriteBatch _spriteBatch;

    private readonly PebbleRenderer re;


    private SceneManager sceneManager;

    public GraphicsTesting()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;


        re = new PebbleRenderer(this, graphics, 1000, 600, 1f, 0.5f);

        sceneManager = new SceneManager(this);

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.Components.Add(sceneManager);
        this.Services.AddService(typeof(ISceneControllerService), sceneManager);

        this.Components.Add(re);
        this.Services.AddService(typeof(IPebbleRendererService), re);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        re.LoadShaders();
        //_spriteBatch = new SpriteBatch(GraphicsDevice);

        Scene myScene = new GraphicsTestScene();
        this.sceneManager.AddScene(myScene);

        this.sceneManager.ChangeScene("GraphicsTestScene");

        // TODO: use this.Content to load your game content here
    }

    private bool done = false;
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!done)
        {
            // sceneManager.DebugPrintGraph();
            done = true;
        }

        // sceneManager.DebugPrintGraph();

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

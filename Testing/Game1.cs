using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Testing;

using GREngine.Core.System;
using SystemTesting;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SceneManager sceneManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        sceneManager = new SceneManager(this);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.Components.Add(sceneManager);
        this.Services.AddService(typeof(ISceneControllerService), sceneManager);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Scene myScene = new MyScene();
        this.sceneManager.AddScene(myScene);

        this.sceneManager.ChangeScene("MyScene");

        // TODO: use this.Content to load your game content here
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
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}

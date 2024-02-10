using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDemo1;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using Scenes;

public class App : Game
{
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private readonly SceneManager sceneManager;
    private readonly PebbleRenderer renderManager;
    private readonly CollisionSystem collisionManager;

    public App()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        sceneManager = new SceneManager(this);
        renderManager = new PebbleRenderer(this, _graphics, 1920, 1080, 1f, 0.5f);
        collisionManager = new CollisionSystem(this);
    }

    protected override void Initialize()
    {
        this.Components.Add(sceneManager);
        this.Services.AddService(typeof(ISceneControllerService), sceneManager);

        this.Components.Add(renderManager);
        this.Services.AddService(typeof(IPebbleRendererService), renderManager);

        this.Components.Add(collisionManager);
        this.Services.AddService(typeof(ICollisionSystem), collisionManager);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        this.sceneManager.AddScene(new GameScene());

        this.sceneManager.ChangeScene("GameScene");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.renderManager.Draw(gameTime);
    }
}

global using static GREngine.Debug.Out;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameDemo1;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using Scenes;
//using Testing.SystemTesting;

public class App : Game
{
    private readonly GraphicsDeviceManager _graphics;

    private readonly SceneManager sceneManager;
    private readonly PebbleRenderer renderManager;
    private readonly CollisionSystem collisionManager;

    public App()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        sceneManager = new SceneManager(this);
        renderManager = new PebbleRenderer(this, _graphics, 1000, 600, 1f, 0.5f);
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
        Material oceanMat = new Material(
            new Shader(Content.Load<Effect>("Graphics/oceanDiffuse")),
            new Shader(Content.Load<Effect>("Graphics/oceanNormal")),
            new Shader(Content.Load<Effect>("Graphics/puddleRoughness")));
        renderManager.addMaterial(oceanMat);
        Material puddleMat = new Material(
            null,
            new Shader(Content.Load<Effect>("Graphics/puddleNormalMapped")),
            new Shader(Content.Load<Effect>("Graphics/puddleRoughness")));
        renderManager.addMaterial(puddleMat);
        Material playerMat = new Material(
            new Shader(Content.Load<Effect>("Graphics/PlayerDiffuseShader")), null, null);
        Material laserMat = new Material(
            new Shader(Content.Load<Effect>("Graphics/LaserShader")), null, null);
        renderManager.addMaterial(playerMat);
        renderManager.addMaterial(laserMat);
        renderManager.addPostProcess(
            new BloomPostProcess(this,
                Content.Load<Effect>("Graphics/isolate"), 1920, 1080, 32, 0.9f));
        renderManager.addPostProcess(
            new DitherPostProcess(this,
                Content.Load<Effect>("Graphics/dither"),
                Content.Load<Texture2D>("Graphics/bayer")));

        //renderManager.addPostProcess(new PostProcess(this, Content.Load<Effect>("Graphics/tonemapping")));
        //renderManager.addPostProcess(new BloomPostProcess(this, 1920, 1080, 1, 0.9f));
        renderManager.addPostProcess(new PostProcess(this, Content.Load<Effect>("Graphics/crtPostProcess")));

        this.sceneManager.AddScene(new GameScene());
        this.sceneManager.AddScene(new DeathScene());

        //this.sceneManager.ChangeScene("GameScene");
        this.sceneManager.ChangeScene("GameScene");

        //this.sceneManager.ChangeScene("GunTestingScene");
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        this.renderManager.Draw(gameTime);
    }
}

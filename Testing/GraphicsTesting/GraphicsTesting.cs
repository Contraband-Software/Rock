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


        re = new PebbleRenderer(this, graphics, 1920, 1080, 0.25f, 0.5f);

        sceneManager = new SceneManager(this);

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        this.Components.Add(sceneManager);
        this.Services.AddService(typeof(ISceneControllerService), sceneManager);

        this.Components.Add(re);
        this.Services.AddService(typeof(IPebbleRendererService), re);
        re.lookAt(new Vector2(512, 512));
        base.Initialize();
    }

    protected override void LoadContent()
    {
        re.LoadShaders();
        Material oceanMat = new Material(new Shader(Content.Load<Effect>("Graphics/oceanDiffuse")), new Shader(Content.Load<Effect>("Graphics/oceanNormal")), new Shader(Content.Load<Effect>("Graphics/puddleRoughness")));
        Material puddleMat = new Material(null, new Shader(Content.Load<Effect>("Graphics/puddleNormalMapped")), new Shader(Content.Load<Effect>("Graphics/puddleRoughness")));

        //Material oceanMat = new Material(new Shader(Content.Load<Effect>("Graphics/oceanDiffuse")), new Shader(Content.Load<Effect>("Graphics/oceanNormal")), new Shader(Content.Load<Effect>("Graphics/oceanRoughness")));
        re.addMaterial(oceanMat);
        re.addMaterial(puddleMat);

        //re.addPostProcess(new PostProcess(this, Content.Load<Effect>("Graphics/tonemapping")));
        re.addPostProcess(new BloomPostProcess(this, Content.Load<Effect>("Graphics/isolate"), 1920, 1080, 32, 0.9f));
        //re.addPostProcess(new BlurPostProcess(this, 1920, 1080, 1, 0.9f));
        re.addPostProcess(new DitherPostProcess(this, Content.Load<Effect>("Graphics/dither"), Content.Load<Texture2D>("Graphics/bayer")));
        //re.addPostProcess(new PostProcess(this, Content.Load<Effect>("Graphics/crtPostProcess")));

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
        re.drawDebug(new DebugDrawable(new Vector2(512, 512), 64, Color.Green));
        //re.drawDebug(new DebugDrawable(new Vector2(1024, 1024), 124, Color.Green));
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

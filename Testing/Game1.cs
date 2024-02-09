using GREngine.Core.PebbleRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Testing;

public class Game1 : Game
{
    private GraphicsDeviceManager graphics;
    //private SpriteBatch _spriteBatch;

    private readonly PebbleRenderer re;


    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        re = new PebbleRenderer(this, graphics, 1920, 1080, 1f, 0.5f);
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here


        this.Components.Add(re);
        this.Services.AddService(typeof(IPebbleRendererService), re);

        base.Initialize();


    }

    protected override void LoadContent()
    {
        re.LoadShaders();   
        //_spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

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

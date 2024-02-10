namespace Testing.CollisionTesting
{
    using GREngine.Core.Physics2D;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Color = Microsoft.Xna.Framework.Color;

    using GREngine.Core.System;
    using Microsoft.Xna.Framework.Graphics;
    using System.Drawing;
    using Microsoft.Xna.Framework.Input;
    using Testing.SystemTesting;
    using System.Diagnostics;
    using GREngine.Core.PebbleRenderer;

    internal class CollisionTesting : Game
    {
        private GraphicsDeviceManager graphics;

        private Texture2D texture;
        private Texture2D pixelTexture;
        private CollisionSystem collisionSystem;
        private SceneManager sceneManager;

        private PebbleRenderer pebbleRenderer;

        public CollisionTesting()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            collisionSystem = new CollisionSystem(this);
            sceneManager = new SceneManager(this);
            pebbleRenderer = new PebbleRenderer(this, graphics, 1280, 720, 1, 1);
        }

        protected override void Initialize()
        {


            // TODO: Add your initialization logic here
            Services.AddService(typeof(ISceneControllerService), sceneManager);
            Services.AddService(typeof(ICollisionSystem), collisionSystem);
            Services.AddService(typeof(IPebbleRendererService), pebbleRenderer);
            Components.Add(sceneManager);
            Components.Add(collisionSystem);
            Components.Add(pebbleRenderer);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            pebbleRenderer.LoadShaders();
            sceneManager.AddScene(new GeneralCollisionTesting());
            sceneManager.ChangeScene("GeneralCollisionTesting");

        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            pebbleRenderer.Draw(gameTime);
            // TODO: Add your drawing code here

            //drawing polygons


            base.Draw(gameTime);
        }
    }
}

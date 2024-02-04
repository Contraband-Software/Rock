namespace LD54.AsteroidGame.GameObjects
{
    using LD54.AsteroidGame.GameObjects;
    using LD54.Engine.Collision;
    using LD54.Engine.Components;
    using LD54.Engine.Leviathan;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public  class Asteroid : GameObject
    {
        public float Mass { get; set; } = 1;
        private Vector3 initialVelocity = Vector3.Zero;

        Texture2D texture;
        Texture2D brokenTexture;

        public float rotationSpeed;

        CircleColliderComponent collider;
        RigidBodyComponent rb;
        SpriteRendererComponent src;

        private float deathCountdownTime = 1f;

        public enum State { ALIVE, DYING};
        public State state { get; private set; } = State.ALIVE;

        float scale;

        public Asteroid(
            float mass,
            Vector3 initialVelocity,
            Texture2D texture,
            Texture2D brokenText,
            string name,
            Game appCtx) : base(name, appCtx)
        {
            this.texture = texture;
            this.brokenTexture = brokenText;

            this.initialVelocity = initialVelocity;
            this.Mass = mass;


            Random rnd = new Random();
            this.scale = 0.8f + (rnd.NextSingle() * 0.2f);

            rotationSpeed = (0.1f + (rnd.NextSingle() * 0.8f)) * (MathF.Sign(rnd.NextSingle()-0.5f));
        }

        public override void OnLoad(GameObject? parentObject)
        {
            src = new SpriteRendererComponent("asteroid", this.app);
            src.LoadSpriteData(
                this.GetGlobalTransform(),
                new Vector2((this.texture.Width * this.scale), (this.texture.Height * this.scale)),
                this.texture
                );
            this.AddComponent(src);

            Vector3 colliderDimensions = new Vector3(
                this.texture.Width * this.scale,
                this.texture.Height * this.scale,
                0);


            // PrintLn(this.rotationSpeed.ToString());
            collider = new CircleColliderComponent(colliderDimensions.X / 2, Vector3.Zero, "asteroidCollider", this.app);
            collider.isTrigger = true;
            this.collider.DebugMode = false;

            this.AddComponent(collider);

            rb = new RigidBodyComponent("rbAsteroid", app);
            rb.Mass = this.Mass;
            rb.Velocity = this.initialVelocity;
            this.AddComponent(rb);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(state == State.ALIVE)
            {
                Rotation += rotationSpeed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                src.Rotation = Rotation;
            }
            if(state == State.DYING)
            {
                deathCountdownTime -= (gameTime.ElapsedGameTime.Milliseconds / 1000f);
                if(deathCountdownTime < 0)
                {
                    this.app.Services.GetService<ISceneControllerService>().DestroyObject(this);
                }
            }

            if(GetGlobalPosition().Length()  > 4000) {
                this.app.Services.GetService<ISceneControllerService>().DestroyObject(this);
            }
        }

        /// <summary>
        /// goes into DYING state and changes to "blown up" version of sprite
        /// </summary>
        public void StartDeathCountdown()
        {
            if(state == State.ALIVE)
            {
                // PrintLn("starting DEATH sequence for asteroid");
                state = State.DYING;
                rotationSpeed = 0;
                src.SetSprite(brokenTexture);
            }
        }
    }
}

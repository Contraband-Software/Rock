namespace LD54.AsteroidGame.GameObjects
{
    using LD54.Engine.Collision;
    using LD54.Engine.Components;
    using LD54.Engine.Leviathan;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework;
    using System;
    using Math = Engine.Math;
    using LD54.AsteroidGame.Scenes;
    using Microsoft.Xna.Framework.Audio;

    public class Spaceship : GameObject
    {
        BlackHole blackHole;
        private ILeviathanEngineService renderer;

        Texture2D texture;
        Texture2D texture_boost;

        CircleColliderComponent collider;
        RigidBodyComponent rb;
        SpriteRendererComponent src;

        public delegate void AsteroidHit();
        public AsteroidHit AsteroidHitEvent;

        public float BlackHoleOrbitRadius { get; protected set; } = 0;

        int light;

        private SoundEffect[] sfx;
        private bool isBoosting = false;

        #region PARAMS
        // PLEASE DO NOT EDIT THESE, ONE THING CHANGES AND EVERYTHING NEEDS TO BE ADJUSTED - ASK SAM
        private const float moveForce = 30f;
        private const float MaxRotationSpeed = 3.5f;
        private const float velocityDamping = 0.88f;
        private const float maxVelocityFactor = 10f;
        private float warmupFactor = 29;
        private const float forceConstant = 0.001f;
        private const float boostFactor = 0.4f;
        private const float fallFactor = 14f;
        private const float deathSpeed = 3;
        public const float MaxRadius = 1000;
        // private float
        #endregion

        public Spaceship(BlackHole blackHole, SoundEffect[] sfx, Texture2D texture, Texture2D boost, string name, Game appCtx) : base(name, appCtx)
        {
            this.blackHole = blackHole;
            this.texture = texture;
            this.texture_boost = boost;
            this.sfx = sfx;
        }

        public override void OnLoad(GameObject? parentObject)
        {
            renderer = this.app.Services.GetService<ILeviathanEngineService>();

            float scaleDivider = 1;

            src = new SpriteRendererComponent("spaceship", this.app);
            src.LoadSpriteData(
                this.GetGlobalTransform(),
                new Vector2((this.texture.Width / scaleDivider), (this.texture.Height / scaleDivider)),
                this.texture
                );
            this.AddComponent(src);

            Vector3 colliderDimensions = new Vector3(this.texture.Width, this.texture.Height, 0);
            collider = new CircleColliderComponent(colliderDimensions.X/2, Vector3.Zero, "playerCollider", this.app);
            collider.isTrigger = true;
            this.collider.DebugMode = true;
            collider.TriggerEvent += OnTriggerEnter;
            this.AddComponent(collider);

            rb = new RigidBodyComponent("rbPlayer", app);
            rb.Mass = 0;
            rb.Velocity += new Vector3(0, -80, 0);
            this.AddComponent(rb);

            light = renderer.AddLight(this.GetGlobalPosition().SwizzleXY(), new Vector3(0, 1, 1) * 200000f);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.isBoosting)
            {
                if ((int)gameTime.TotalGameTime.TotalMilliseconds % 40 == 0)
                {
                    SoundEffectInstance? eSFX = this.sfx[(int)GameScene.GAME_SFX.ENGINE].CreateInstance();
                    eSFX.Volume = 0.3f;
                    eSFX.Play();
                }
            }


            #region TOY_ORBIT_PHYSICS
            float distanceToBlackHole = 0;
            {
                var render = this.app.Services.GetService<ILeviathanEngineService>();

                Vector3 blackHolePosition = blackHole.GetGlobalPosition();
                Vector3 shipPosition = this.GetGlobalPosition();

                // Distance to black hole
                Vector2 positionDelta = new Vector2(
                    blackHolePosition.X - shipPosition.X,
                    blackHolePosition.Y - shipPosition.Y
                );
                float r = positionDelta.Magnitude();
                distanceToBlackHole = r;
                BlackHoleOrbitRadius = r;

                Vector2 orbitTangent = positionDelta.PerpendicularCounterClockwise();
                float tangentVelocity = Vector2.Dot(this.rb.Velocity.SwizzleXY(), orbitTangent) / orbitTangent.Length();

                Vector2 orbitNormal = positionDelta.RNormalize();
                Vector3 accelerationToCenter = new Vector3(
                    orbitNormal * MathF.Abs(tangentVelocity) * r * (gameTime.ElapsedGameTime.Milliseconds / 1000f), 0) * forceConstant;

                orbitTangent.Normalize();
                Vector3 finalAbsoluteVelocity = accelerationToCenter + new Vector3((tangentVelocity) * orbitTangent, 0);

                #region ROTATION
                {
                    float a = finalAbsoluteVelocity.SwizzleXY().Length();
                    float b = r;
                    float c = r;

                    float totalRotation = MathF.Acos(
                        (b * b + c * c - (a * a)) / (2 * b * c)
                    );

                    float rot = totalRotation * MathF.Sign(tangentVelocity) * -1;

                    if (float.IsFinite(rot)) this.Rotation += rot;
                }
                #endregion

                // More speed = more radius increase per frame
                float speedAbs = this.rb.Velocity.Copy().Length();
                finalAbsoluteVelocity += new Vector3(-orbitNormal, 0) * boostFactor * speedAbs;

                // Always being pulled in
                finalAbsoluteVelocity += new Vector3(orbitNormal, 0) * fallFactor * MathF.Abs(speedAbs == 0f ? 1f : (1 / MathF.Pow(speedAbs, 0.7f))); // change this final float to change how fast the player needs to keep going
                Vector3 slowdown = new Vector3(orbitTangent * -tangentVelocity, 0).RNormalize() * fallFactor * (1 / (distanceToBlackHole < 0.1f ? 0.0000001f : MathF.Pow(distanceToBlackHole, 0.75f))); // change this final float to affect how much more powerful the blackhole suckage gets when you get closer
                finalAbsoluteVelocity += slowdown;

                //if (speedAbs < deathSpeed)
                //{
                //   finalAbsoluteVelocity += new Vector3(orbitNormal, 0) * maxVelocityFactor * (deathSpeed - speedAbs);
                //}

                if (Math.IsFinite(finalAbsoluteVelocity)) this.rb.Velocity = finalAbsoluteVelocity;

                // Debug stuff, no more math after here
                // Vector2 overlapOffset = new Vector2(1, 1) * 10;
                float velScale = 50;

                render.DebugDrawCircle(new Vector2(blackHolePosition.X, blackHolePosition.Y), r, Color.Lime);
                render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY()  + orbitNormal                  * 100, Color.Crimson);
                render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY() , this.GetGlobalPosition().SwizzleXY() + this.rb.Velocity.SwizzleXY() * velScale, Color.Cyan);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY() + positionDelta, Color.Pink);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY() + slowdown.SwizzleXY() * velScale * 100, Color.Pink);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY() + finalAbsoluteVelocity.SwizzleXY(), Color.Pink);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY() + tangentVelocity * velScale * orbitTangent, Color.Cyan);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY(), this.GetGlobalPosition().SwizzleXY() + tangent, Color.Cyan);
                // render.DebugDrawLine(this.GetGlobalPosition().SwizzleXY() + overlapOffset, this.GetGlobalPosition().SwizzleXY() + overlapOffset + positionDelta * 140, Color.Lime);


                {

                    Vector3 blackHolePosition2 = blackHole.GetGlobalPosition();
                    Vector3 shipPosition2 = this.GetGlobalPosition();

                    // Distance to black hole
                    Vector2 positionDelta2 = new Vector2(
                        blackHolePosition2.X - shipPosition2.X,
                        blackHolePosition2.Y - shipPosition2.Y
                    );
                    float r2 = positionDelta.Magnitude();

                    if (r2 > MaxRadius)
                    {
                        this.SetLocalPosition(this.GetLocalPosition() + new Vector3(positionDelta.RNormalize() * (r2 - MaxRadius), 0));
                    }
                }
            }
            #endregion
            Vector2 movementVector = Move(gameTime);
            //limit velocity
            if (rb.Velocity.Length() > maxVelocityFactor) //(1 - 1 / (d == 0 ? 1 : d)) *
            {
                rb.Velocity = rb.Velocity.RNormalize() * maxVelocityFactor;
            }
            #region TOY_ORBIT_PHYSICS_CONT
            {
                // gonna do summin here wit movementVector
            }
            #endregion

            renderer.SetCameraPosition(
                new Vector2(
                    this.GetGlobalPosition().X + texture.Width / 2,
                    this.GetGlobalPosition().Y + texture.Height / 2
                    ) - renderer.getWindowSize() / 2
                );

            base.Update(gameTime);
            src.Rotation = Rotation;

            renderer.SetLightPosition(light, this.GetGlobalPosition().SwizzleXY());

            System.Random rnd = new Random();
            if (this.rb.Velocity.Length() < 0.001f) this.rb.Velocity += new Vector3(rnd.Next(), rnd.Next(), 0);
        }

        private void RotateLeft(GameTime gameTime)
        {
            Rotation -= MaxRotationSpeed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
        }
        private void RotateRight(GameTime gameTime)
        {
            Rotation += MaxRotationSpeed * (gameTime.ElapsedGameTime.Milliseconds / 1000f);
        }

        private void OnTriggerEnter(ColliderComponent other)
        {
            // PrintLn("TRIGGERED ON: " + other.GetName());

            //see if gameobject collided with is of type Asteroid
            //if so, start its death countdown
            if (other.GetGameObject() is Asteroid)
            {
                Asteroid asteroid = (Asteroid)other.GetGameObject();
                if (asteroid != null)
                {
                    if (asteroid.state == Asteroid.State.ALIVE)
                    {
                        asteroid.StartDeathCountdown();
                        //get velocity of asteroid
                        Vector3 asteroidVel = ((RigidBodyComponent)asteroid.GetComponent<RigidBodyComponent>()).Velocity;

                        //get position delta from asteroid to player
                        Vector3 asteroidPos = asteroid.GetGlobalPosition();
                        Vector3 thisPos = GetGlobalPosition();
                        Vector3 posDelta = new Vector3(
                            thisPos.X - asteroidPos.X,
                            thisPos.Y - asteroidPos.Y,
                            0);

                        float dotProduct = Vector3.Dot(asteroidVel, posDelta);
                        /*if (dotProduct > 0)
                        {
                            rb.Velocity = asteroidVel * 5f;
                        }

                        //otherwise, damp current velocity
                        else
                        {
                            rb.Velocity *= 0.0f;
                        }*/
                        posDelta.Normalize();
                        rb.Velocity += posDelta * 500f;
                        enginePower = 0;
                        warmupFactor += 15f;
                        this.sfx[(int)GameScene.GAME_SFX.HIT].Play();
                        this.isBoosting = false;
                        AsteroidHitEvent();
                    }
                }
            }
        }

        /// <summary>
        /// Calculate and return the direction this object is facing in
        /// </summary>
        /// <returns>direction as a unit vector</returns>
        private Vector2 ForwardVector()
        {
            float x = MathF.Sin(Rotation);
            float y = -MathF.Cos(Rotation);
            Vector2 directionVector = new Vector2(x, y);
            return directionVector;
        }

        private float enginePower = 0;
        private Vector2 MoveInForwardDirection(GameTime gameTime)
        {
            Vector2 directionVector = ForwardVector();
            Vector2 forceVector = directionVector * moveForce * (gameTime.ElapsedGameTime.Milliseconds / 1000f);

            this.enginePower += (1 - this.enginePower) / warmupFactor;
            //PrintLn(this.warmupFactor.ToString());
            rb.Velocity += new Vector3(forceVector, 0) * this.enginePower;

            return forceVector;
        }

        private Vector2 Move(GameTime gameTime)
        {
            Vector2 forceVector = Vector2.Zero;
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                MoveInForwardDirection(gameTime);
                src.SetSprite(texture_boost);

                this.isBoosting = true;
            }
            else
            {
                this.enginePower = 0;
                src.SetSprite(texture);

                this.isBoosting = false;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                RotateLeft(gameTime);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                RotateRight(gameTime);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                //rb.Velocity.Y += Speed;
                // this.app.Services.GetService<ISceneControllerService>().ReloadCurrentScene();
            }

            return forceVector;
        }
    }
}

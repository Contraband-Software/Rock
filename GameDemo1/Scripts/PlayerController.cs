namespace GameDemo1.Scripts;

using System;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using GREngine.Algorithms;
using GREngine.Core.Physics2D;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using static GREngine.Debug.Out;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using System.Drawing;
using System.Collections.Generic;
using GameDemo1.Scenes;
using System.Threading;

[GRETagWith("Player")]
public class Player : Node {}

[GRExecutionOrder(10)]
public class PlayerController : Behaviour
{

    Sprite laser;
    int animcounter = 0;
    #region EVENTS
    public delegate void PlayerDied();
    public event PlayerDied? PlayerDiedEvent;
    #endregion

    #region SETTINGS
    private static float walkSpeed = 3;
    private static float maxGunPower = 50;
    private static float gunCooldown = 0.3f;
    private static float gunKnockback = 2;
    private static float platformSpeedDamping = 0.78f;

    private string mapFloorCollisionLayer;
    #endregion

    #region STATE
    private IPebbleRendererService render;
    private Collider rb;

    private int currentGunPower = (int)(maxGunPower / 2);
    private bool isGrounded = true;
    private int scrollDelta = 0;
    private float currentGunCooldown = 0;
    private float facingDirection = 0;
    private float maxFallTime = 1;

    private float Size
    {
        get
        {
            return (this.currentFallTime != 0) ? this.maxFallTime / this.currentFallTime : 1;
        }
    }

    #region INTERNAL
    private bool gunFiredThisFrame = false;
    private int previousScrollValue = 0;
    private float currentFallTime = 0;
    #endregion
    #endregion

    public PlayerController(string mapFloorCollisionLayer = "mapFloor")
    {
        this.mapFloorCollisionLayer = mapFloorCollisionLayer;
    }

    protected override void OnAwake()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
    }

    protected override void OnStart()
    {
        rb = this.Node.GetBehaviour<Collider>() as Collider;

        this.rb.OnTriggerEnter += c =>
        {
            if (c.GetLayer() == mapFloorCollisionLayer)
            {
                this.isGrounded = true;

                (c.Node.GetBehaviour<EnemySpawner>() as EnemySpawner).PlayerFrameTouch();
            }
        };


        Texture2D allWhite = new Texture2D(Game.GraphicsDevice, 1, 1);
        allWhite.SetData(new Color[] { Color.White });



        laser = new Sprite(0, new Vector2(0, currentGunPower * 2), allWhite, null, null, 6, 4);
        //laser.offset = new Vector2(0, 100);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(Node, laser);

        Sprite playerSprite = new Sprite(0, new Vector2(0.2f), Game.Content.Load<Texture2D>("Graphics/PlayerDiffuse"), null, null, 6, 3);
        this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(Node, playerSprite);
    }

    protected override void OnUpdate(GameTime gameTime)
    {

        if (animcounter <= 0)
        {
            laser.setScale(new Vector2(0, currentGunPower * 2));
        }
        else
        {
            animcounter--;
        }
        #region INPUT
        this.ManageMouseInput();

        Vector2 m = this.ManageMovementInput();
        if (isGrounded && m.Length() > 0)
        {
            this.rb.Velocity += m * walkSpeed;
        }

        this.ManageGunInput(gameTime);
        #endregion

        if (this.isGrounded)
        {
            currentFallTime = 0;
            this.rb.Velocity *= platformSpeedDamping;
        }
        else
        {
            currentFallTime += gameTime.ElapsedGameTime.Milliseconds / 1000f;

            if (currentFallTime > this.maxFallTime)
            {
                GameOver();
            }
        }
        isGrounded = false;

        this.Game.Services.GetService<IPebbleRendererService>()
            .lookAt(this.Node.GetGlobalPosition2D());// - new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2));

        // this.Game.Services.GetService<ISceneControllerService>().DebugPrintGraph();
    }

    private void GameOver()
    {
        PlayerDiedEvent?.Invoke();
        this.Game.Services.GetService<ISceneControllerService>().ChangeScene("DeathScene");
        // PrintLn("Game Over");
    }

    private void FireGun()
    {
        ICollisionSystem collisionSystem = this.Game.Services.GetService<ICollisionSystem>();
        this.rb.Velocity += (Vector.AngleToVector(-this.facingDirection) * this.currentGunPower * gunKnockback);

        
        

        Vector2 direction = Vector.AngleToVector(this.facingDirection);
        List<string> layers = new List<string>
        {
            GameScene.enemyCollisionLayer
        };
        PointF origin = new PointF(Node.GetGlobalPosition().X, Node.GetGlobalPosition().Y);

        direction.Y = direction.Y * -1;
        if (direction != Vector2.Zero) { direction.Normalize(); }
        Vector2 normal1 = new Vector2(-direction.Y, direction.X);
        Vector2 normal2 = new Vector2(direction.Y, -direction.X);
        PrintLn(direction.ToString());

        PointF origin2 = new PointF(origin.X + (normal1.X * 10f), origin.Y + (normal1.Y * 10f));
        PointF origin3 = new PointF(origin.X + (normal2.X * 10f), origin.Y + (normal2.Y * 10f));

        Raycast2DResult ray = collisionSystem.Raycast2D(origin, direction, 100000f, layers);
        Raycast2DResult ray2 = collisionSystem.Raycast2D(origin2, direction, 100000f, layers);
        Raycast2DResult ray3 = collisionSystem.Raycast2D(origin3, direction, 100000f, layers);

        Collider hitCollider = null;
        if(ray.colliderHit != null) { hitCollider = ray.colliderHit; }
        else if(ray2.colliderHit != null) { hitCollider = ray2.colliderHit; }
        else if(ray3.colliderHit != null) { hitCollider = ray3.colliderHit; }

        if(hitCollider != null)
        {
            float distanceToHitCollider = new Vector2(
                hitCollider.GetGlobalColliderPosition().X - origin.X,
                hitCollider.GetGlobalColliderPosition().Y - origin.Y).Length();

            Vector2 originVector = new Vector2(origin.X, origin.Y);
            Vector2 beamEndPoint = originVector + (direction * distanceToHitCollider);

            Enemy e = (hitCollider.Node.GetBehaviour<Enemy>() as Enemy);


            Vector2 v = direction * currentGunPower * 2f;
            e.Node.SetLocalPosition(e.Node.GetLocalPosition2D() + v);
        }


        Texture2D allWhite = new Texture2D(Game.GraphicsDevice, 1, 1);
        allWhite.SetData(new Color[] { Color.White });

        //Sprite laser = new Sprite(MathF.Sin(direction.X), new Vector2(4000, currentGunPower * 2), allWhite, null, null,6,4);
        //this.Game.Services.GetService<ISceneControllerService>().AddBehaviour(Node, laser);
        laser.rotation = MathF.Atan2(direction.Y,direction.X);
        laser.setScale(new Vector2(3000, currentGunPower * 2));
        animcounter = 60;
    }

    private void ManageGunInput(GameTime gameTime)
    {
        currentGunPower = (int)Math.Clamp(this.currentGunPower + this.scrollDelta / 30, 0, maxGunPower);

        if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            if (this.currentGunCooldown == 0)
            {
                this.currentGunCooldown = gunCooldown;
                this.FireGun();
            }
        }

        if (this.currentGunCooldown > 0)
        {
            this.currentGunCooldown -= (gameTime.ElapsedGameTime.Milliseconds / 1000f);
            if (this.currentGunCooldown < 0)
                this.currentGunCooldown = 0;
        }
    }

    private Vector2 ManageMovementInput()
    {
        Vector2 forceVector = Vector2.Zero;
        if (Keyboard.GetState().IsKeyDown(Keys.W))
        {
            forceVector += new Vector2(0, -1);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.A))
        {
            forceVector += new Vector2(-1, 0);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.D))
        {
            forceVector += new Vector2(1, 0);
        }
        if (Keyboard.GetState().IsKeyDown(Keys.S))
        {
            forceVector += new Vector2(0, 1);
        }

        if (forceVector.Length() > 0) forceVector.Normalize();

        return forceVector;
    }

    private void ManageMouseInput()
    {
        // Scroll wheel
        MouseState currentMouseState = Mouse.GetState();
        scrollDelta = currentMouseState.ScrollWheelValue - previousScrollValue;
        previousScrollValue = currentMouseState.ScrollWheelValue;

        // Aim direction
        Vector2 relativeTo = new Vector2(
            this.Game.GraphicsDevice.Viewport.Width  / 2,
            this.Game.GraphicsDevice.Viewport.Height / 2);
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        Vector2 mouseDisplacement = mousePos - relativeTo;
        if (mouseDisplacement.Length() != 0) mouseDisplacement.Normalize();
        this.facingDirection = Vector.VectorToAngle(mouseDisplacement);
    }
}

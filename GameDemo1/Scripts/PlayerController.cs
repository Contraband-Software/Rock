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

[GRETagWith("Player")]
public class Player : Node {}

[GRExecutionOrder(10)]
public class PlayerController : Behaviour
{
    #region EVENTS
    public delegate void PlayerDied();
    public event PlayerDied? PlayerDiedEvent;
    #endregion

    #region SETTINGS
    private float walkSpeed = 2;
    private float maxGunPower = 60;
    private float gunCooldown = 0.5f;
    private float gunKnockback = 1;
    private float platformSpeedDamping = 0.68f;

    private string mapFloorCollisionLayer;
    #endregion

    #region STATE
    private IPebbleRendererService render;
    private Collider rb;

    private int currentGunPower = 0;
    private bool isGrounded = true;
    private int scrollDelta = 0;
    private float currentGunCooldown = 0;
    private float facingDirection = 0;
    private float maxFallTime = 3;

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
            }
        };
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        #region INPUT
        this.ManageMouseInput();

        Vector2 m = this.ManageMovementInput();
        if (isGrounded && m.Length() > 0)
        {
            this.rb.Velocity += m * this.walkSpeed;
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

        render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), 20, Color.Orange));

        this.Game.Services.GetService<IPebbleRendererService>()
            .setCameraPosition(this.Node.GetGlobalPosition2D() - new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2));
    }

    private void GameOver()
    {
        PlayerDiedEvent?.Invoke();
        // PrintLn("Game Over");
    }

    private void FireGun()
    {
        this.rb.Velocity += (Vector.AngleToVector(-this.facingDirection) * this.currentGunPower * gunKnockback);
    }

    private void ManageGunInput(GameTime gameTime)
    {
        currentGunPower = (int)Math.Clamp(this.currentGunPower + this.scrollDelta / 30, 0, maxGunPower);

        if (Keyboard.GetState().IsKeyDown(Keys.Space))// || Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            if (this.currentGunCooldown == 0)
            {
                this.currentGunCooldown = this.gunCooldown;
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

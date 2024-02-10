namespace GameDemo1.Scripts;

using System;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using GREngine.Algorithms;
using GREngine.Core.Physics2D;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using static GREngine.Debug.Out;
using Vector2 = Microsoft.Xna.Framework.Vector2;

[GRETagWith("Player")]
public class Player : Node {}

public class PlayerController : Behaviour
{
    #region EVENTS
    public delegate void PlayerDied();
    public event PlayerDied? PlayerDiedEvent;
    #endregion

    #region SETTINGS
    private float walkSpeed = 10;
    private float maxGunPower = 60;
    private float gunCooldown = 0.5f;
    private float gunKnockback = 1;
    #endregion

    #region STATE
    private IPebbleRendererService render;
    private Collider rb;

    private int currentGunPower = 0;
    private bool isGrounded = true;
    private int scrollDelta = 0;
    private float currentGunCooldown = 0;
    private float facingDirection = 0;

    #region INTERNAL
    private bool gunFiredThisFrame = false;
    private int previousScrollValue = 0;
    #endregion
    #endregion

    protected override void OnAwake()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
    }

    protected override void OnStart()
    {
        rb = this.Node.GetBehaviour<Collider>() as Collider;
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        #region INPUT
        this.ManageMouseInput();

        Vector2 m = this.ManageMovementInput();
        if (isGrounded && m.Length() > 0)
        {
            Node.SetLocalPosition((Node.GetLocalPosition2D() + m * this.walkSpeed));
        }

        this.ManageGunInput(gameTime);
        #endregion

        // PrintLn(this.Node.GetGlobalPosition().ToString());

        render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), 20, Color.Orange));
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
                PrintLn("pew");
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

        forceVector.Normalize();

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
        mouseDisplacement.Normalize();
        this.facingDirection = Vector.VectorToAngle(mouseDisplacement);
    }
}

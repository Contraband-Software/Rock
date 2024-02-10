namespace GameDemo1.Scripts;

using System;
using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using GREngine.Debug;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

[GRETagWith("Player")]
public class Player : Node {}

public class PlayerController : Behaviour
{
    #region EVENTS
    public delegate void PlayerDied();
    public event PlayerDied? PlayerDiedEvent;
    #endregion

    #region SETTINGS
    private float speed = 10;
    private float maxGunPower = 60;
    private float gunCooldown = 0.5f;
    #endregion

    #region STATE
    private IPebbleRendererService render;

    private int currentGunPower = 0;
    private bool isGrounded = true;
    private int scrollDelta = 0;
    private float currentGunCooldown = 0;

    #region INTERNAL
    private bool gunFiredThisFrame = false;
    private int previousScrollValue = 0;
    #endregion
    #endregion

    protected override void OnAwake()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        #region INPUT
        this.ManageScrollInput();

        Vector2 m = this.ManageMovementInput();
        if (isGrounded && m.Length() > 0)
        {
            Node.SetLocalPosition((Node.GetLocalPosition2D() + m * this.speed));
        }

        this.ManageGunInput(gameTime);
        #endregion

        render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), 20, Color.Orange));
    }

    private void FireGun()
    {

    }

    private void ManageGunInput(GameTime gameTime)
    {
        currentGunPower = (int)Math.Clamp(this.currentGunPower + this.scrollDelta / 30, 0, maxGunPower);

        if (Keyboard.GetState().IsKeyDown(Keys.Space) || Mouse.GetState().LeftButton == ButtonState.Pressed)
        {
            if (this.currentGunCooldown == 0)
            {
                Out.PrintLn("fuck");
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

    private void ManageScrollInput()
    {
        MouseState currentMouseState = Mouse.GetState();
        scrollDelta = currentMouseState.ScrollWheelValue - previousScrollValue;
        previousScrollValue = currentMouseState.ScrollWheelValue;
    }
}

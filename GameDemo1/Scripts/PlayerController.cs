namespace GameDemo1.Scripts;

using GREngine.Core.PebbleRenderer;
using GREngine.Core.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

public class PlayerController : Behaviour
{
    private IPebbleRendererService render;

    protected override void OnAwake()
    {
        render = this.Game.Services.GetService<IPebbleRendererService>();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        Node.SetLocalPosition(Node.GetLocalPosition2D() + this.Move() * 10);

        render.drawDebug(new DebugDrawable(this.Node.GetLocalPosition2D(), 20, Color.Orange));
    }

    private Vector2 Move()
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
}

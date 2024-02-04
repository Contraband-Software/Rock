namespace LD54.Engine.Components;

using Microsoft.Xna.Framework;
using System;

public class RigidBodyComponent : Component
{
    public Vector3 Velocity = Vector3.Zero;
    public float Mass = 1;
    public bool Static = false;

    public RigidBodyComponent(string name, Game appCtx) : base(name, appCtx)
    {
    }

    public override void OnLoad(GameObject? parentObject)
    {
        this.gameObject = parentObject;
    }

    public override void Update(GameTime gameTime)
    {
        if (!this.Enabled) return;

        base.Update(gameTime);

        if (this.Static) return;
        this.gameObject.SetLocalPosition(this.gameObject.GetLocalPosition() + this.Velocity);
    }

    public override void OnUnload()
    {

    }

    /// <summary>
    /// Adds force to the rigidbody in a direction
    /// </summary>
    /// <param name="force">force vector</param>
    public void AddForce(Vector2 force)
    {
        Vector2 acceleration = (force / this.Mass);
        this.Velocity += new Vector3(acceleration.X, acceleration.Y, 0);
    }
}

using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using GREngine.Core.Physics2D;
using System.Collections.Generic;

namespace GREngine.Core.Physics2D;

public abstract class VerletObject
{

    private Vector2 currentPosition = Vector2.Zero;
    private Vector2 oldPosition = Vector2.Zero;
    private Vector2 acceleration = Vector2.Zero;
    private float rotation;
    private AABB aabb;
    private bool aabbOverlapping = false;

    private Vector2 externalVelocity = Vector2.Zero;
    private bool isStatic = false;

    public VerletObject(Vector2 initialPosition)
    {
        currentPosition = initialPosition;
        oldPosition = initialPosition;
    }

    public void updatePosition(float dt)
    {
        rotation %= 360; //ensure rotation never goes above 360 (resets to 0)

        Vector2 velocity = currentPosition - oldPosition;
        // save current position
        oldPosition = new Vector2(currentPosition.X, currentPosition.Y);
        // Perform verlet integration
        currentPosition = currentPosition + velocity + acceleration * dt * dt;
        // Reset acceleration
        acceleration = Vector2.Zero;
    }

    public void SetVelocity(Vector2 externalVelocity)
    {
        if (isStatic) { return; }
        // Set the velocity externally
        float m = externalVelocity.Length() * 1;
        Vector2 n = externalVelocity;
        n.Normalize();
        Vector2 v = m * n;

        currentPosition = new Vector2(oldPosition.X + v.X, oldPosition.Y + v.Y);
    }

    public void accelerate(Vector2 acc)
    {
        acceleration = acc;
    }

    public Vector2 GetPosition() { return currentPosition; }
    public Vector2 GetOldPosition() { return oldPosition; }

    public void SetPosition(Vector2 position)
    {
        //oldPosition = currentPosition;
        currentPosition = position;
        //oldPosition = currentPosition;

    }

    public void SetRotation(float rotation)
    {
        this.rotation = rotation;
    }

    public float GetRotation()
    {
        return rotation;
    }

    public void SolveCollision(VerletObject other, Vector2 velocity)
    {
        if (other is CircleCollider circCol)
        {
            SolveCollision(circCol, velocity);
        }
        else if (other is PolygonCollider polyCol)
        {
            SolveCollision(polyCol, velocity);
        }
    }

    public AABB GetAABB() { return aabb; }
    protected void SetAABB(AABB aabb) { this.aabb = aabb; }
    public void SetStatic(bool b) { isStatic = b; }
    public bool IsStatic() { return isStatic; }
    public bool IsAABBOverlapping() { return aabbOverlapping; }
    public void SetAABBOverlapping(bool b) { aabbOverlapping = b; }

    public void SolveCollisions(List<VerletObject> others)
    {
        SetAABBOverlapping(true);
        Vector2 velocityVector = new Vector2(GetPosition().X - GetOldPosition().X, GetPosition().Y - GetOldPosition().Y);
        foreach (VerletObject other in others)
        {
            other.SetAABBOverlapping(true);
            SolveCollision(other, velocityVector);
        }
    }
    public abstract void SolveCollision(PolygonCollider other, Vector2 velocity);
    public abstract void SolveCollision(CircleCollider other, Vector2 velocity);

    /// <summary>
    /// Each subclass will have its own method of calculating its AABB
    /// </summary>
    public virtual void CalculateAABB() { }

    /// <summary>
    /// Define how to draw the verlet object for debugging
    /// (eg, draw the collider outline)
    /// </summary>
    public virtual void DrawDebug(SpriteBatch spriteBatch, Texture2D pixelTexture) { }
}

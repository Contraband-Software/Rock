using System;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using GREngine.Core.Physics2D;
using System.Collections.Generic;
using GREngine.Core.System;
using System.Xml.Linq;
using GREngine.Core.PebbleRenderer;
using System.Drawing;
using GREngine.Debug;

namespace GREngine.Core.Physics2D;

using Debug;

public abstract class Collider : Behaviour
{
    private Vector2 oldPosition = Vector2.Zero;
    private Vector2 acceleration = Vector2.Zero;
    private float rotation;
    private AABB aabb;
    private bool aabbOverlapping = false;

    protected ICollisionSystem collisionSystem;

    private Vector2 offset = Vector2.Zero;
    private bool isStatic = false;
    private bool isTrigger = false;
    private string collisionLayer = "default";

    public delegate void TriggerEvent(Collider collidedWith);
    public event TriggerEvent? OnTriggerEnter;
    public delegate void CollisionEvent(Collider collidedWith);
    public event CollisionEvent? OnCollisionEnter;

    List<string> allowedCollisionLayers = new List<string>();

    public bool Debug { get; set; } = false;

    public float VelocityDampingMultiplier { get; set; } = 1;

    public Vector2 Velocity
    {
        get => GetLocalNodePosition() - oldPosition;
        set => SetVelocity(value);
    }

    public Collider()
    {
    }

    public Collider(Vector2 offset, string layer, bool debug)
    {
        this.offset = offset;
        this.collisionLayer = layer;
        Debug = debug;
    }
    public Collider(string layer, bool debug)
    {
        this.offset = Vector2.Zero;
        this.collisionLayer = layer;
        Debug = debug;
    }

    public Collider(bool debug)
    {
        this.offset = Vector2.Zero;
        this.collisionLayer = "default";
        Debug = debug;
    }

    public Collider(Vector2 offset, bool debug)
    {
        this.offset = offset;
        this.collisionLayer = "default";
        Debug = debug;
    }

    protected override void OnAwake()
    {
        collisionSystem = this.Game.Services.GetService<ICollisionSystem>();
        EnabledChangedEvent += state =>
        {
            if (state)
            {
                this.collisionSystem.AddCollisionObject(this);
            }
            else
            {
                this.collisionSystem.RemoveCollisionObject(this);
            }
        };
    }

    protected override void OnStart()
    {
        oldPosition = GetLocalNodePosition();

        this.collisionSystem.AddCollisionObject(this);
    }

    protected override void OnDestroy()
    {
        this.Game.Services.GetService<ICollisionSystem>().RemoveCollisionObject(this);
    }

    protected override void OnUpdate(GameTime gameTime)
    {
#if DEBUG
        if (Debug)
        {
            //this.DrawDebug();
        }
#endif
        this.Velocity *= this.VelocityDampingMultiplier;
    }

    public void updatePosition(float dt)
    {
        rotation %= 360; //ensure rotation never goes above 360 (resets to 0)

        Vector2 currentPosition = GetLocalNodePosition();
        Vector2 velocity = currentPosition - oldPosition;
        // save current position
        oldPosition = new Vector2(currentPosition.X, currentPosition.Y);
        // Perform verlet integration
        SetNodePosition(currentPosition + velocity + acceleration);
        // Reset acceleration
        acceleration = Vector2.Zero;
    }

    public void SetVelocity(Vector2 externalVelocity)
    {
        if (isStatic) { return; }
        // Set the velocity externally
        float m = externalVelocity.Length() * 1;
        Vector2 n = externalVelocity;
        if (n.Length() > 0) n.Normalize();
        Vector2 v = m * n;

        SetNodePosition(new Vector2(oldPosition.X + v.X, oldPosition.Y + v.Y));
    }

    public void accelerate(Vector2 acc)
    {
        acceleration = acc;
    }
    public Vector2 GetOldPosition() { return oldPosition; }

    /// <summary>
    /// Gets local position from the node as a Vector2
    /// This method is used for velocity
    /// </summary>
    public Vector2 GetLocalNodePosition()
    {
        Vector3 pos = Node.GetLocalPosition();
        return new Vector2(pos.X, pos.Y);
    }

    public void SetFuckingOldPosition(Vector2 yourmother)
    {
        this.oldPosition = yourmother;
    }

    /// <summary>
    /// Gets global position from the node as a Vector2
    /// This method is used for collision detection
    /// </summary>
    /// <returns></returns>
    public Vector2 GetGlobalColliderPosition()
    {
        Vector3 pos = Node.GetGlobalPosition();
        return new Vector2(pos.X, pos.Y);
    }

    /// <summary>
    /// Sets the position of the Node
    /// </summary>
    /// <param name="position"></param>
    public void SetNodePosition(Vector2 position)
    {
        float z = Node.GetLocalPosition().Z;
        Vector3 pos = new Vector3(position.X, position.Y, z);
        Node.SetLocalPosition(pos);
    }
    public Vector2 GetOffset()
    {
        return offset;
    }
    public void SetOffset(Vector2 offset) { this.offset = offset; }
    public float GetRotation()
    {
        return rotation;
    }
    public void SetRotation(float r)
    {
        rotation = r;
    }

    public AABB GetAABB() { return aabb; }
    protected void SetAABB(AABB aabb) { this.aabb = aabb; }
    public void SetStatic(bool b) { isStatic = b; }
    public bool IsStatic() { return isStatic; }
    public void SetTrigger(bool b) { isTrigger = b; }
    public bool IsTrigger() { return isTrigger;}
    public void SetLayer(string layer) { collisionLayer = layer; }
    public string GetLayer() { return collisionLayer; }
    public bool IsAABBOverlapping() { return aabbOverlapping; }
    public void SetAABBOverlapping(bool b) { aabbOverlapping = b; }

    /// <summary>
    /// Pass in a list of layers with which this collider should be able to collide with
    /// </summary>
    /// <param name="layers"></param>
    public void SetAllowedCollisionLayers(List<string> layers)
    {
        this.allowedCollisionLayers = layers;
    }
    public List<string> GetAllowedCollisionLayers() { return allowedCollisionLayers; }

    public void SolveCollisions(List<Collider> others)
    {
        SetAABBOverlapping(true);
        Vector2 velocityVector = new Vector2(GetLocalNodePosition().X - GetOldPosition().X, GetLocalNodePosition().Y - GetOldPosition().Y);
        foreach (Collider other in others)
        {
            other.SetAABBOverlapping(true);
            SolveCollision(other, velocityVector);
        }
    }
    public void SolveCollision(Collider other, Vector2 velocity)
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
    public abstract void SolveCollision(PolygonCollider other, Vector2 velocity);
    public abstract void SolveCollision(CircleCollider other, Vector2 velocity);

    public abstract bool PointInsideCollider(PointF point);

    /// <summary>
    /// Each subclass will have its own method of calculating its AABB
    /// </summary>
    public abstract void CalculateAABB();

    /// <summary>
    /// Define how to draw the verlet object for debugging
    /// (eg, draw the collider outline)
    /// </summary>
    public virtual void DrawDebug() { }

    protected void FireCorrectEvent(Vector2 collisionVector, Collider other)
    {
        if(collisionVector == Vector2.Zero)
        {
            return;
        }
        if (IsTrigger() || other.IsTrigger())
        {
            FireTriggerEnter(collisionVector, other);
            other.FireTriggerEnter(collisionVector, this);
        }
        else if (other.IsStatic())
        {
            FireCollisionEnter(collisionVector, other);
            other.FireCollisionEnter(collisionVector, this);
        }
        else
        {
            FireCollisionEnter(collisionVector, other);
            other.FireCollisionEnter(collisionVector, this);
        }
    }

    protected void FireTriggerEnter(Vector2 collisionVector, Collider other)
    {
        if(collisionVector != Vector2.Zero)
        {
            this.OnTriggerEnter?.Invoke(other);
        }
    }
    protected void FireCollisionEnter(Vector2 collisionVector, Collider other)
    {
        if (collisionVector != Vector2.Zero)
        {
            this.OnCollisionEnter?.Invoke(other);
        }
    }
}

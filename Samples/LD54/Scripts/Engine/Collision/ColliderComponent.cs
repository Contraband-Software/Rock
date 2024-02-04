namespace LD54.Engine.Collision;

using LD54.Engine;
using LD54.Engine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Drawing;
using Leviathan;
using Color = Microsoft.Xna.Framework.Color;

public abstract class ColliderComponent : Component
{
    #region EVENTS
    public delegate void Trigger(ColliderComponent collidedWith);
    public event Trigger TriggerEvent;
    public void InvokeTriggerEvent(ColliderComponent collidedWith)
    {
        if (TriggerEvent is not null) TriggerEvent(collidedWith);
    }

    public delegate void Collide(ColliderComponent collidedWith);
    public event Collide CollideEvent;
    public void InvokeCollideEvent(ColliderComponent collidedWith)
    {
        if (CollideEvent is not null) CollideEvent(collidedWith);
    }

    #endregion

    private int colliderID;
    public Vector3 previousPosition;
    public bool isTrigger;

    public ColliderComponent(string name, Game appCtx) : base(name, appCtx)
    {
    }

    public override void OnLoad(GameObject? parentObject)
    {
        this.gameObject = parentObject;
        this.app.Services.GetService<ICollisionSystemService>().AddColliderToSystem(this);
    }

    public override void OnUnload()
    {
        this.app.Services.GetService<ICollisionSystemService>().RemoveColliderFromSystem(this);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
}

public class CircleColliderComponent : ColliderComponent
{
    public bool DebugMode = false;
#if _DEBUG || DEBUG
    private void ShowBoundsIfDebug()
    {
        if (this.DebugMode) this.app.Services.GetService<ILeviathanEngineService>().DebugDrawCircle(this.centre, this.radius, Color.Lime);
    }
#else
    private void DebugShowBounds() {}
#endif

    public Vector2 centre; //this is the equivalent of the aabb
    public float radius;
    private Vector3 offset;

    public CircleColliderComponent(float radius, Vector3 offset, string name, Game appCtx) : base(name, appCtx)
    {
        this.radius = radius;
        this.offset = offset;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        base.OnLoad(parentObject);
        RecalculateCentre();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        RigidBodyComponent rb = (RigidBodyComponent)gameObject.GetComponent<RigidBodyComponent>();
        if (rb != null)
        {
            previousPosition = (gameObject.GetGlobalPosition() + offset) - rb.Velocity;
        }
        else
        {
            previousPosition = gameObject.GetGlobalPosition() + offset;
        }
        RecalculateCentre();

        //ShowBoundsIfDebug();
    }

    public override void OnUnload()
    {
        base.OnUnload();
    }

    public void RecalculateCentre()
    {
        Vector3 colliderOrigin = gameObject.GetGlobalPosition() + offset;
        centre = new Vector2(colliderOrigin.X + radius, colliderOrigin.Y + radius);
    }
}

public class BoxColliderComponent : ColliderComponent
{
    public AABB aabb;
    private Vector3 dimensions;
    private Vector3 offset;

    public BoxColliderComponent(Vector3 dimensions, Vector3 offset, string name, Game appCtx) : base(name, appCtx)
    {
        this.dimensions = dimensions;
        this.offset = offset;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        base.OnLoad(parentObject);
        RecalculateAABB();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        RigidBodyComponent rb = (RigidBodyComponent)gameObject.GetComponent<RigidBodyComponent>();
        if (rb != null)
        {
            previousPosition = (gameObject.GetGlobalPosition() + offset) - rb.Velocity;
        }
        else
        {
            previousPosition = gameObject.GetGlobalPosition() + offset;
        }

        //update aabb per update to match where gameObject is
        RecalculateAABB();
    }

    public void RecalculateAABB()
    {
        Vector3 colliderOrigin = gameObject.GetGlobalPosition() + offset;
        Vector3 min = new Vector3(colliderOrigin.X, colliderOrigin.Y + dimensions.Y, colliderOrigin.Z);
        Vector3 max = new Vector3(colliderOrigin.X + dimensions.X, colliderOrigin.Y, colliderOrigin.Z);
        this.aabb.min = min;
        this.aabb.max = max;
    }

}

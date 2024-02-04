namespace LD54.Engine.Collision;

using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Engine.Components;
using Engine.Dev;

public interface ICollisionSystemService
{
    public void AddColliderToSystem(ColliderComponent spriteCollider);
    public void RemoveColliderFromSystem(ColliderComponent spriteCollider);
}

public class CollisionSystem : GameComponent, ICollisionSystemService
{
    private List<ColliderComponent> collisionSystemList;

    public CollisionSystem(Game game) : base(game)
    {
        collisionSystemList = new List<ColliderComponent>();
    }

    public void AddColliderToSystem(ColliderComponent spriteCollider)
    {
        collisionSystemList.Add(spriteCollider);
        //PrintLn("Collider added, count: " + collisionSystemList.Count.ToString());
    }

    public void RemoveColliderFromSystem(ColliderComponent spriteCollider)
    {
        collisionSystemList.Remove(spriteCollider);
    }



    public override void Update(GameTime gameTime)
    {
        //check collision between each collider

        //for now, if theres an overlap, calculate the overlap and
        //give it back to the sprite that you are checking collision for

        //later, will also need to invoke OnCollisionEnter event on a collider
        foreach(ColliderComponent col in collisionSystemList.ToList())
        {
            if(col is BoxColliderComponent)
            {
                CalculateForBoxCollision((BoxColliderComponent)col);
            }
            else if(col is CircleColliderComponent)
            {
                CalculateForCircleCollision((CircleColliderComponent)col);
            }

        }
    }
    /// <summary>
    /// Calculates collisions for given CIRCLE COLLIDER
    /// </summary>
    /// <param name="col"></param>
    private void CalculateForCircleCollision(CircleColliderComponent collider)
    {
        collider.RecalculateCentre();
        List<Collision> collisions = new List<Collision>();
        for(int i = 0; i < collisionSystemList.Count; i++)
        {
            ColliderComponent other = collisionSystemList[i];

            if (other == null || other == collider || other is BoxColliderComponent) continue;
            CircleColliderComponent ot = (CircleColliderComponent)other;

            Overlap collision = TestCircleOverlap(collider, ot);
            if (collision.isOverlap)
            {
                collisions.Add(new Collision(other, collision));
            }
        }
        RequestResolveForCircle(collider, collisions);
    }

    private Overlap TestCircleOverlap(CircleColliderComponent a, CircleColliderComponent b)
    {
        Vector2 posDelta = new Vector2(b.centre.X - a.centre.X, b.centre.Y - a.centre.Y);
        float distance = MathF.Abs(posDelta.Length());
        float radiusSum = a.radius + b.radius;
        if(distance < radiusSum)
        {
            return new Overlap(true, posDelta, radiusSum - distance);
        }

        return new Overlap(false);
    }

    private void RequestResolveForCircle(CircleColliderComponent requestingCollider, List<Collision> collisions)
    {
        GameObject requestingColliderObj = requestingCollider.GetGameObject();
        RigidBodyComponent requestingColliderRb = (RigidBodyComponent)requestingColliderObj.GetComponent<RigidBodyComponent>();

        //if it has a rigidbody, allow resolution, if not, it is forced to be a trigger
        //also wont resolve if its a static

        foreach(Collision col in collisions)
        {
            //collided with a trigger
            if (col.collider.isTrigger)
            {
                //invoke trigger event
                requestingCollider.InvokeTriggerEvent(col.collider);
                continue;
            }
            //otherwise collided with collider.
            requestingCollider.InvokeCollideEvent(col.collider);

            //dont resolve if marked as static or a trigger
            if (requestingColliderRb == null || requestingCollider.isTrigger || requestingColliderRb.Static)
            {
                return;
            }

            //this ensures that the other collider is not moved
            if (requestingColliderRb.Velocity.Length() == 0f)
            {
                continue;
            }

            Vector2 distanceToAdjustBy = Vector2.Normalize(col.overlap.posDelta) * col.overlap.overlapDistance;
            Vector3 position = requestingColliderObj.GetGlobalTransform().Translation;
            position -= new Vector3(distanceToAdjustBy.X, distanceToAdjustBy.Y, 0);
            position -= requestingCollider.GetGameObject().GetParent().GetGlobalPosition();
            requestingCollider.GetGameObject().SetLocalPosition(position);
        }
    }

    /// <summary>
    /// Calculates collisions for given BOX COLLIDER
    /// </summary>
    /// <param name="collider"></param>
    private void CalculateForBoxCollision(BoxColliderComponent collider)
    {
        collider.RecalculateAABB();
        AABB a = collider.aabb;
        List<Collision> collisions = new List<Collision>();

        for (int i = 0; i < collisionSystemList.Count; i++)
        {
            ColliderComponent other = collisionSystemList[i];
            if (other == null || other == collider || other is CircleColliderComponent) continue;

            BoxColliderComponent ot = (BoxColliderComponent)other;

            AABB b = ot.aabb;
            Overlap collision = TestAABBOverlap(a, b);
            if (collision.isOverlap)
            {
                collisions.Add(new Collision(other, collision));
            }

        }
        //resolve collisions for current collider
        RequestResolveForBox(collider, collisions);
    }


    /// <summary>
    /// Force a recalculation for a collider
    /// </summary>
    public void RequestResolveForBox(BoxColliderComponent requestingCollider, List<Collision> collisions) {


        GameObject requestingColliderObj = requestingCollider.GetGameObject();
        RigidBodyComponent requestingColliderRb = (RigidBodyComponent)requestingColliderObj.GetComponent<RigidBodyComponent>();

        //if it has a rigidbody, allow resolution, if not, it is forced to be a trigger
        if (requestingColliderRb == null || requestingColliderRb.Static)
        {
            //invoke triggerEnter
            return;
        }

        Vector3 position = requestingColliderObj.GetGlobalTransform().Translation;
        foreach (Collision collision in collisions)
        {
            //collided with a trigger
            if (collision.collider.isTrigger)
            {
                //invoke trigger event
                requestingCollider.InvokeTriggerEvent(collision.collider);
                continue;
            }
            //otherwise collided with collider.
            requestingCollider.InvokeCollideEvent(collision.collider);

            BoxColliderComponent colCollider = (BoxColliderComponent)collision.collider;
            //PrintLn(position.ToString());
            //resolve collision
            //overlap box is smaller of d1/2x, d1/2y
            float overlapX = MathF.Min(
                MathF.Abs(collision.overlap.overlaps[0]),
                MathF.Abs(collision.overlap.overlaps[2]));
            float overlapY = MathF.Min(
                MathF.Abs(collision.overlap.overlaps[1]),
                MathF.Abs(collision.overlap.overlaps[3]));

            if(requestingColliderRb.Velocity.X != 0 && requestingColliderRb.Velocity.Y != 0)
            {
                if (overlapX > overlapY)
                {
                    position.Y -= overlapY * MathF.Sign(requestingColliderRb.Velocity.Y);
                    //PrintLn("Collision resolve: 1");
                }
                else if(overlapX < overlapY)
                {
                    position.X -= overlapX * MathF.Sign(requestingColliderRb.Velocity.X);
                    //PrintLn("Collision resolve: 2");
                }
                else{
                    //if left or right of other object (gapX/combined width > gapY/combined height)
                    float widthTarget = requestingCollider.aabb.max.X - requestingCollider.aabb.min.X;
                    float widthCollision = colCollider.aabb.max.X - colCollider.aabb.min.X;

                    float gapRatioX = MathF.Abs(colCollider.aabb.min.X - requestingCollider.previousPosition.X)
                        / (widthTarget + widthCollision);

                    float heightTarget = requestingCollider.aabb.min.Y - requestingCollider.aabb.max.Y;
                    float heightCollision = colCollider.aabb.min.Y - colCollider.aabb.max.Y;

                    float gapRatioY = MathF.Abs(colCollider.aabb.max.Y - requestingCollider.previousPosition.Y)
                        / (heightTarget + heightCollision);

                    //on left or right (maintain Y, resolve X)
                    if(gapRatioX > gapRatioY)
                    {
                        position.X -= (overlapX) * MathF.Sign(requestingColliderRb.Velocity.X);
                        //PrintLn("Collision resolve: 3");
                    }
                    //above or below (maintain X, resolve Y)
                    else
                    {
                        position.Y -= (overlapY) * MathF.Sign(requestingColliderRb.Velocity.Y);
                        //PrintLn("Collision resolve: 4");
                    }
                }
            }
            else
            {
                //PrintLn("Collision resolve: 5");

                if (overlapX > overlapY)
                {
                    position.Y -= overlapY * MathF.Sign(requestingColliderRb.Velocity.Y);
                }
                else if (overlapX < overlapY)
                {
                    position.X -= overlapX * MathF.Sign(requestingColliderRb.Velocity.X);
                }
                else
                {
                    position.Y -= overlapY * MathF.Sign(requestingColliderRb.Velocity.Y);
                    position.X -= overlapX * MathF.Sign(requestingColliderRb.Velocity.X);
                }
            }
            position -= requestingCollider.GetGameObject().GetParent().GetGlobalPosition();
            requestingCollider.GetGameObject().SetLocalPosition(position);

        }
    }

    /// <summary>
    /// Return whether two aabb's are overlapping
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private Overlap TestAABBOverlap(AABB a, AABB b)
    {
        float d1x = b.min.X - a.max.X;
        float d1y = b.min.Y - a.max.Y;
        float d2x = a.min.X - b.max.X;
        float d2y = a.min.Y - b.max.Y;

        if (d1x > 0.0f || d1y < 0.0f)
        {
            return new Overlap(false);
        }

        if (d2x > 0.0f || d2y < 0.0f)
        {
            return new Overlap(false);
        }

        return new Overlap(true, new float[4] { d1x, d1y, d2x, d2y });
    }
}


using GREngine.Core.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GREngine.Core.Physics2D;

using Debug;
using PebbleRenderer;
using Color = Microsoft.Xna.Framework.Color;

public class CircleCollider : Collider
{
    float radius;

    public CircleCollider(float radius) : base()
    {
        this.radius = radius;
    }

    public CircleCollider(float radius, bool debugged) : base(debug: debugged)
    {
        this.radius = radius;
    }

    public CircleCollider(float radius, string collisionLayer, bool debugged=false) : base(layer: collisionLayer, debug: debugged)
    {
        this.radius = radius;
    }
    public CircleCollider(float radius, Vector2 offset, string collisionLayer, bool debugged = false) : base(layer: collisionLayer, debug: debugged, offset:offset)
    {
        this.radius = radius;
    }
    public CircleCollider(float radius, Vector2 offset, bool debugged = false) : base(debug: debugged, offset: offset)
    {
        this.radius = radius;
    }

    public override void DrawDebug()
    {
        Vector2 pos = GetGlobalColliderPosition();
        Game.Services.GetService<IPebbleRendererService>().drawDebug(new DebugDrawable(pos, this.GetRadius(), Color.Lime));
    }

    /// <summary>
    /// Circle Colliding potentially with another circle
    /// </summary>
    /// <param name="obj2"></param>
    ///
    public override void SolveCollision(PolygonCollider other, Vector2 velocity)
    {
        //throw new NotImplementedException();
    }

    public override void SolveCollision(CircleCollider obj2, Vector2 velocity)
    {
        Vector2 collisionAxis = GetGlobalColliderPosition() - obj2.GetGlobalColliderPosition();
        float dist = collisionAxis.Length();
        float minDist = GetRadius() + obj2.GetRadius();
        if (dist < minDist)
        {
            FireCorrectEvent(collisionAxis, obj2);

            Vector2 n = collisionAxis / dist;
            float delta = minDist - dist;

            if (IsTrigger() || obj2.IsTrigger())
            {
                return;
            }
            else if (obj2.IsStatic())
            {

                SetNodePosition(GetGlobalColliderPosition() + 1f * n * delta);
            }
            else
            {
                SetNodePosition(GetGlobalColliderPosition() + 0.5f * n * delta);
                obj2.SetNodePosition(obj2.GetGlobalColliderPosition() - 0.5f * n * delta);
            }
          
        }
    }

    public void ResolveCollision(CircleCollider other, Vector2 collisionVector)
    {
        FireCorrectEvent(collisionVector, other);

        Vector2 currentPos = GetGlobalColliderPosition();
        Vector2 otherCurrentPos = other.GetGlobalColliderPosition();

        Vector2 thisColliderResolution = new Vector2(collisionVector.X * -1, collisionVector.Y * -1);
        if (IsTrigger() || other.IsTrigger())
        {
            return;
        }
        //normal to static, resolve only normal
        else if (other.IsStatic())
        {
            Vector2 resolvedPosition = new Vector2(currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
            SetNodePosition(resolvedPosition);
        }
        //Normal to normal, resolve both
        else
        {
            float thisColliderResolutionScale = thisColliderResolution.Length() * 0.5f;

            if (thisColliderResolution != Vector2.Zero)
            {
                thisColliderResolution.Normalize();
                thisColliderResolution *= thisColliderResolutionScale;
            }

            Vector2 otherColliderResolution = collisionVector;
            float otherColliderResolutionScale = otherColliderResolution.Length() * 0.5f;
            if (otherColliderResolution != Vector2.Zero)
            {
                otherColliderResolution.Normalize();
                otherColliderResolution *= otherColliderResolutionScale;
            }

            Vector2 resolvedPosition = new Vector2(currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
            Vector2 otherColliderResolvedPosition = new Vector2(
                otherCurrentPos.X + otherColliderResolution.X,
                otherCurrentPos.Y + otherColliderResolution.Y);
            SetNodePosition(resolvedPosition);
            other.SetNodePosition(otherColliderResolvedPosition);
        }
    }

    public override bool PointInsideCollider(PointF point)
    {
        float distToCentreSqrd = (GetGlobalColliderPosition() - new Vector2(point.X, point.Y)).LengthSquared();
        return distToCentreSqrd < (radius * radius);
    }

    public float GetRadius()
    {
        return radius;
    }

    public override void CalculateAABB()
    {
        Vector2 centre = GetGlobalColliderPosition();
        PointF min = new PointF(centre.X - radius, centre.Y - radius);
        PointF max = new PointF(centre.X + radius, centre.Y + radius);
        SetAABB(new AABB(min, max));
    }
}

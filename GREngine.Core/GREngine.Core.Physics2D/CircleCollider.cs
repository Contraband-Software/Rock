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
        throw new NotImplementedException();
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
            SetNodePosition(GetLocalNodePosition() + 0.5f * n * delta);
            obj2.SetNodePosition(obj2.GetLocalNodePosition() - 0.5f * n * delta);
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
}

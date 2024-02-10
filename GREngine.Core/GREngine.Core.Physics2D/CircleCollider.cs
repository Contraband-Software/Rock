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

public class CircleCollider : Collider
{
    float radius;

    public CircleCollider(float radius) : base()
    {
        this.radius = radius;
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
        Vector2 collisionAxis = GetGlobalPosition() - obj2.GetGlobalPosition();
        float dist = collisionAxis.Length();
        float minDist = GetRadius() + obj2.GetRadius();
        if (dist < minDist)
        {
            Vector2 n = collisionAxis / dist;
            float delta = minDist - dist;
            SetPosition(GetPosition() + 0.5f * n * delta);
            obj2.SetPosition(obj2.GetPosition() - 0.5f * n * delta);
        }
    }

    public override bool PointInsideCollider(PointF point)
    {
        float distToCentreSqrd = (GetGlobalPosition() - new Vector2(point.X, point.Y)).LengthSquared();
        return distToCentreSqrd < (radius * radius);
    }

    public float GetRadius()
    {
        return radius;
    }
}

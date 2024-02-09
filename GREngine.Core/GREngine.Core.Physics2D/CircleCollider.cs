using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GREngine.Core.Physics2D;

public class CircleCollider : VerletObject
{
    float radius;

    public CircleCollider(Vector2 initialPosition, float radius) : base(initialPosition)
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
        Vector2 collisionAxis = GetPosition() - obj2.GetPosition();
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

    public float GetRadius()
    {
        return radius;
    }
}

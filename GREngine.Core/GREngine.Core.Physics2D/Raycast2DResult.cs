using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace GREngine.Core.Physics2D;
/// <summary>
/// Stores the result of a Raycast2D
/// </summary>
public struct Raycast2DResult
{
    public Collider colliderHit;
    public PointF hitPoint;
    public Vector2 collisionNormal;
    public Raycast2DResult(Collider colliderHit, PointF hitPoint, Vector2 collisionNormal)
    {
        this.colliderHit = colliderHit;
        this.hitPoint = hitPoint;
        this.collisionNormal = collisionNormal;
    }
}

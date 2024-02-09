using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace GREngine.Core.Physics2D;

public interface ICollisionSystem
{
}
//This should be a singleton service
public class CollisionSystem : ICollisionSystem
{
    HashSet<VerletObject> verletObjects = new HashSet<VerletObject>();
    Vector2 gravity = new Vector2(0, 1000f);
    Vector2 position = new Vector2(300, 0);
    float radius = 300f;

    int subSteps = 1;

    // Singleton Pattern
    public static CollisionSystem instance;
    private CollisionSystem()
    {
    }
    public static CollisionSystem Instance
    {
        get
        {
            if (instance == null)
            {
                if (instance == null)
                {
                    instance = new CollisionSystem();
                }
            }

            return instance;
        }
    }

    public void Update(float dt)
    {
        float subDt = dt / (float)subSteps;
        for (int i = 0; i < subSteps; i++)
        {
            applyGravity();
            applyConstraint();
            SolveCollisions();
            updatePositions(subDt);
        }
    }

    void applyGravity()
    {
        foreach (VerletObject obj in verletObjects.OfType<CircleCollider>())
        {
            obj.accelerate(gravity);
        }
        foreach (VerletObject obj in verletObjects.OfType<PolygonCollider>())
        {
            if (!obj.IsStatic())
            {
                //obj.accelerate(gravity);
            }
        }
    }

    void applyConstraint()
    {
        foreach (CircleCollider obj in verletObjects.OfType<CircleCollider>())
        {
            Vector2 to_obj = obj.GetPosition() - position;
            float dist = to_obj.Length();
            if (dist > (radius - obj.GetRadius()))
            {
                Vector2 normalized = to_obj / dist;
                obj.SetPosition(position + normalized * (radius - obj.GetRadius()));
            }
        }
    }

    void SolveCollisions()
    {
        //discrete collision calculation
        //brute force approach
        int objCount = verletObjects.Count;
        foreach (VerletObject obj1 in verletObjects)
        {
            if (obj1.IsStatic()) { continue; }

            obj1.SetAABBOverlapping(false);
            obj1.CalculateAABB();
            foreach (VerletObject obj2 in verletObjects)
            {
                obj2.CalculateAABB();
                if (obj1 == obj2)
                {
                    continue;
                }
                obj1.SolveCollision(obj2);
            }
        }
    }

    #region CALCULATIONS
    /// <summary>
    /// Tests whether two AABB's overlap
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool AABBOverlap(AABB a, AABB b)
    {
        bool xOverlap = a.min.X <= b.max.X && a.max.X >= b.min.X;
        bool yOverlap = a.min.Y <= b.max.Y && a.max.Y >= b.min.Y;

        // If there is overlap along both axes, then the AABBs overlap
        return xOverlap && yOverlap;
    }
    /// <summary>
    /// Creates a new AABB that represents the AABB that the two given AABB's cover
    /// (AABB Union)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static AABB GetCombinedAABBRegion(AABB a, AABB b)
    {
        PointF newMin = new PointF(
            Math.Min(a.min.X, b.min.X),
            Math.Min(a.min.Y, b.min.Y));

        PointF newMax = new PointF(
            Math.Max(a.max.X, b.max.X),
            Math.Max(a.max.Y, b.max.Y));

        return new AABB(newMin, newMax);
    }
    /// <summary>
    /// Creates a new AABB that represents where 2 given AABB's overlap
    /// (AABB Intersection)
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static AABB GetAABBOverlapRegion(AABB a, AABB b)
    {
        PointF newMin = new PointF(
            Math.Max(a.min.X, b.min.X),
            Math.Max(a.min.Y, b.min.Y));

        PointF newMax = new PointF(
            Math.Min(a.max.X, b.max.X),
            Math.Min(a.max.Y, b.max.Y));

        return new AABB(newMin, newMax);
    }

    public static bool PointIsInAABB(PointF p, AABB aabb)
    {
        return (p.X <= aabb.max.X && p.X >= aabb.min.X &&
            p.Y <= aabb.max.Y && p.Y >= aabb.min.Y);
    }

    /// <summary>
    /// Checks whether the AABB's of two line segments overlap
    /// (hence potential intersection)
    /// </summary>
    /// <param name="a1">Origin of line 1</param>
    /// <param name="a2">end of line 1</param>
    /// <param name="b1">Origin of line 2</param>
    /// <param name="b2">end of line 2</param>
    /// <returns></returns>
    public static bool LinesSegmentsOverlap(PointF a1, PointF a2, PointF b1, PointF b2)
    {
        if (!(Math.Max(a1.X, a2.X) >= Math.Min(b1.X, b2.X) && Math.Max(b1.X, b2.X) >= Math.Min(a1.X, a2.X) &&
            Math.Max(a1.Y, a2.Y) >= Math.Min(b1.Y, b2.Y) && Math.Max(b1.Y, b2.Y) >= Math.Min(a1.Y, a2.Y)))
        {
            // Bounding boxes do not intersect
            return false;
        }

        return true;
    }

    /// <summary>
    /// This simply checks if two Lines can intersect since their gradients are not the same
    /// </summary>
    /// <param name="l1">Primary Line</param>
    /// <param name="l2">Other colliders line</param>
    /// <returns></returns>
    public static bool LinesCanIntersect(Line l1, Line l2)
    {
        return l1.m != l2.m;
    }

    /// <summary>
    /// Will take two lines that will intersect, and finds their intersection point.
    /// </summary>
    /// <param name="l1">Primary Line</param>
    /// <param name="l2">Other colliders line</param>
    /// <returns></returns>
    public static PointF LineIntersectionPoint(Line l1, Line l2)
    {
        float x, y;

        if (l1.a == 0f)
        {
            // This line is vertical
            x = -l1.c;
            y = l2.m * x + l2.c;
        }
        else if (l2.a == 0f)
        {
            // Other line is vertical
            x = -l2.c;
            y = l1.m * x + l1.c;
        }
        else
        {
            x = (l2.c - l1.c) / (l1.m - l2.m);
            y = l1.m * x + l1.c;
        }

        return new PointF(x, y);
    }
    /// <summary>
    /// Checks whether passed in point falls between the bounds of both lines
    /// to ensure that an intersection occurred inside the segments
    /// </summary>
    /// /// <param name="intersection"></param>
    /// <param name="a1">Origin of line 1</param>
    /// <param name="a2">end of line 1</param>
    /// <param name="b1">Origin of line 2</param>
    /// <param name="b2">end of line 2</param>
    /// <returns></returns>
    public static bool IntersectionIsWithinLineSegments(PointF intersection, PointF a1, PointF a2, PointF b1, PointF b2)
    {
        float x = intersection.X;
        float y = intersection.Y;
        if ((x >= Math.Min(a1.X, a2.X) && x <= Math.Max(a1.X, a2.X) &&
            y >= Math.Min(a1.Y, a2.Y) && y <= Math.Max(a1.Y, a2.Y)) &&
            (x >= Math.Min(b1.X, b2.X) && x <= Math.Max(b1.X, b2.X) &&
            y >= Math.Min(b1.Y, b2.Y) && y <= Math.Max(b1.Y, b2.Y)))
        {
            return true;
        }
        return false;
    }
    #endregion


    void updatePositions(float dt)
    {
        foreach (VerletObject obj in verletObjects)
        {
            obj.updatePosition(dt);
        }
    }

    public void AddCollisionObject(VerletObject obj)
    {
        verletObjects.Add(obj);
    }
    public HashSet<VerletObject> GetVerletObjects() { return verletObjects; }

    public void SetPosition(Vector2 position)
    {
        this.position = position;
    }

    public Vector2 GetPosition()
    {
        return position;
    }
    public float GetRadius()
    {
        return radius;
    }
}

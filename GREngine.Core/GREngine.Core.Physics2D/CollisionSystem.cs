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
using System.Xml;

namespace GREngine.Core.Physics2D;

public interface ICollisionSystem
{
    public bool AABBOverlap(AABB a, AABB b);
    public AABB GetCombinedAABBRegion(AABB a, AABB b);
    public AABB GetAABBOverlapRegion(AABB a, AABB b);
    public bool PointIsInAABB(PointF p, AABB aabb);
    public bool LinesSegmentsOverlap(PointF a1, PointF a2, PointF b1, PointF b2);
    public bool LinesCanIntersect(Line l1, Line l2);
    public PointF LineIntersectionPoint(Line l1, Line l2);
    public List<PointF> LineIntersectsCircle(PointF p1, PointF p2, PointF c, float r);
    public bool IntersectionIsWithinLineSegments(PointF intersection, PointF a1, PointF a2, PointF b1, PointF b2);
    public void AddCollisionObject(Collider obj);
    public void RemoveCollisionObject(Collider obj);
    public HashSet<Collider> GetColliderObjects();
    public bool PointIsCollidingWithLayer(PointF point, string layer);
    public List<Collider> GetCollidersOfLayer(string layer);

    public Raycast2DResult Raycast2D(PointF origin, Vector2 direction, float distance, List<string> layers);
}
public class CollisionSystem : GameComponent, ICollisionSystem
{
    HashSet<Collider> verletObjects = new HashSet<Collider>();
    Vector2 gravity = new Vector2(0, 1000f);

    int subSteps = 1;
    public CollisionSystem(Game game) : base(game){}

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float subDt = dt / (float)subSteps;
        for (int i = 0; i < subSteps; i++)
        {
            //applyGravity();
            SolveCollisions();
            updatePositions(subDt);
        }
    }

    void applyGravity()
    {
        foreach (Collider obj in verletObjects.OfType<CircleCollider>())
        {
            obj.accelerate(gravity);
        }
        foreach (Collider obj in verletObjects.OfType<PolygonCollider>())
        {
            if (!obj.IsStatic())
            {
                //obj.accelerate(gravity);
            }
        }
    }

    void SolveCollisions()
    {
        //discrete collision calculation
        //brute force approach
        int objCount = verletObjects.Count;
        foreach (Collider obj1 in verletObjects)
        {
            if (obj1.IsStatic()) { continue; }

            obj1.SetAABBOverlapping(false);
            obj1.CalculateAABB();

            List<Collider> aabbOverlapObjects = new List<Collider>();
            foreach (Collider obj2 in verletObjects)
            {
                if(!obj1.GetAllowedCollisionLayers().Contains(obj2.GetLayer())) { continue; }

                obj2.CalculateAABB();
                if (obj1 == obj2)
                {
                    continue;
                }
                if(AABBOverlap(obj1.GetAABB(), obj2.GetAABB()))
                {
                    aabbOverlapObjects.Add(obj2);
                }
            }

            obj1.SolveCollisions(aabbOverlapObjects);
            obj1.DrawDebug();
        }
    }

    #region CALCULATIONS
    /// <summary>
    /// Tests whether two AABB's overlap
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public bool AABBOverlap(AABB a, AABB b)
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
    public AABB GetCombinedAABBRegion(AABB a, AABB b)
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
    public AABB GetAABBOverlapRegion(AABB a, AABB b)
    {
        PointF newMin = new PointF(
            Math.Max(a.min.X, b.min.X),
            Math.Max(a.min.Y, b.min.Y));

        PointF newMax = new PointF(
            Math.Min(a.max.X, b.max.X),
            Math.Min(a.max.Y, b.max.Y));

        return new AABB(newMin, newMax);
    }

    public bool PointIsInAABB(PointF p, AABB aabb)
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
    public bool LinesSegmentsOverlap(PointF a1, PointF a2, PointF b1, PointF b2)
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
    public bool LinesCanIntersect(Line l1, Line l2)
    {
        return (l1.a == 1 && l2.a == 1 && l1.m != l2.m) || (l1.a != l2.a);
    }

    /// <summary>
    /// Will take two lines that will intersect, and finds their intersection point.
    /// </summary>
    /// <param name="l1">Primary Line</param>
    /// <param name="l2">Other colliders line</param>
    /// <returns></returns>
    public PointF LineIntersectionPoint(Line l1, Line l2)
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
    public bool IntersectionIsWithinLineSegments(PointF intersection, PointF a1, PointF a2, PointF b1, PointF b2)
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

    /// <summary>
    /// Return whether a line segment intersect a circle in TWO PLACES
    /// </summary>
    /// <param name="p1"></param>
    /// <param name="p2"></param>
    /// <param name="c"></param>
    /// <param name="r"></param>
    /// <returns></returns>
    public bool LineCanIntersectCircle(PointF p1, PointF p2, PointF c, float r)
    {
        p1 = new PointF(p1.X - c.X, p1.Y - c.Y);
        p2 = new PointF(p2.X - c.X, p2.Y - c.Y);

        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        float dr = MathF.Sqrt((dx * dx) + (dy * dy));
        float D = (p1.X * p2.Y) - (p2.X * p1.Y);

        return ((r * r) * (dr * dr) - (D * D) > 0);
    }

    public List<PointF> LineIntersectsCircle(PointF p1, PointF p2, PointF c, float r)
    {
        p1 = new PointF(p1.X - c.X, p1.Y - c.Y);
        p2 = new PointF(p2.X - c.X, p2.Y - c.Y);

        float dx = p2.X - p1.X;
        float dy = p2.Y - p1.Y;
        float dr = MathF.Sqrt((dx * dx) + (dy * dy));
        float D = (p1.X * p2.Y) - (p2.X * p1.Y);

        float x1 = ((D * dy) + (Math.Sign(dy) * dx * MathF.Sqrt(((r * r) * (dr * dr)) - (D * D)))) / (dr * dr);
        float x2 = ((D * dy) - (Math.Sign(dy) * dx * MathF.Sqrt(((r * r) * (dr * dr)) - (D * D)))) / (dr * dr);

        float y1 = ((-1 * D * dx) + (Math.Abs(dy) * MathF.Sqrt(((r * r) * (dr * dr)) - (D * D)))) / (dr * dr);
        float y2 = ((-1 * D * dx) - (Math.Abs(dy) * MathF.Sqrt(((r * r) * (dr * dr)) - (D * D)))) / (dr * dr);

        return new List<PointF> {
            new PointF(x1 + c.X, y1 + c.Y),
            new PointF(x2 + c.X, y2 + c.X) };
    }

    void updatePositions(float dt)
    {
        foreach (Collider obj in verletObjects)
        {
            obj.updatePosition(dt);
        }
    }

    public void AddCollisionObject(Collider obj)
    {
        verletObjects.Add(obj);
    }
    public void RemoveCollisionObject(Collider obj) { verletObjects.Remove(obj);}
    public HashSet<Collider> GetColliderObjects() { return verletObjects; }

    public bool PointIsCollidingWithLayer(PointF point, string layer)
    {
        foreach(Collider obj in verletObjects)
        {
            if(obj.GetLayer() == layer)
            {
                if (obj.PointInsideCollider(point))
                { return true; }
            }
        }
        return false;
    }

    public List<Collider> GetCollidersOfLayer(string layer)
    {
        return this.verletObjects.Where(c => c.GetLayer() == layer).ToList();
    }

    /// <summary>
    /// Casts a ray from a point in given direction for given distance.
    /// Will stop at the first collider hit
    /// </summary>
    /// <param name="origin">Point the raycast starts</param>
    /// <param name="direction">direction in which it is cast</param>
    /// <param name="distance">max distance the ray will extend</param>
    /// <param name="layers">the layers the ray is allowed to collide with</param>
    /// <returns>A Raycast2DResult that stores the collider hit reference, hit point, and collision normal</returns>
    public Raycast2DResult Raycast2D(PointF origin, Vector2 direction, float distance, List<string> layers)
    {
        //initialise list of all first intersection points
        // have parellel list of what colliders they correspond to
        // and another parellel list of the corresponding normal
        List<PointF> intersectionPoints = new List<PointF>();
        List<Collider> hitColliders = new List<Collider>();
        List<Vector2> normals = new List<Vector2>();

        // get all colliders where the AABB overlaps with the raycast
        List<Collider> possibleColliders = new List<Collider>();

        direction = GREngine.Algorithms.Vector.SafeNormalize(direction);
        PointF endPoint = new PointF(origin.X + direction.X * distance, origin.Y + direction.Y * distance);
        PointF min = new PointF(Math.Min(origin.X, endPoint.X), Math.Min(origin.Y, endPoint.Y));
        PointF max = new PointF(Math.Max(origin.X, endPoint.X), Math.Max(origin.Y, endPoint.Y));
        AABB rayAABB = new AABB(min, max);

        Line rayAsLine = new Line(origin, endPoint);

        //collect colliders within AABB overlap region of ray, and of allowed layer
        foreach(Collider collider in verletObjects)
        {
            collider.CalculateAABB();
            if (!layers.Contains(collider.GetLayer())){ continue; }
            if(!AABBOverlap(rayAABB, collider.GetAABB())) { continue; }

            //RAYCASTS CANT BE FIRED FROM WITHIN A COLLIDER
            if (collider.PointInsideCollider(origin))
            {
                return new Raycast2DResult(
                null,
                new PointF(float.NaN, float.NaN),
                new Vector2(float.NaN, float.NaN));
            }
            possibleColliders.Add(collider);
        }

        //for all polygon colliders, find all points of intersection (if any), and store the closest one
        foreach(Collider obj in verletObjects.OfType<PolygonCollider>())
        {
            PolygonCollider polyCol = (PolygonCollider)obj;
            List<PointF> vertices = polyCol.GetVertices();

            //for all lines in the collider:
            List<PointF> foundIntersectionPoints = new List<PointF>();
            List<Vector2> foundIntersectionPointNormals = new List<Vector2>();
            for (int i = 0; i < vertices.Count; i++)
            {
                PointF b1 = vertices[i];
                PointF b2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];
                //check if ray intersects this collider line
                if (!LinesSegmentsOverlap(origin, endPoint, b1, b2))
                {
                    continue;
                }
                Line colliderLine = new Line(b1, b2);

                //check if ray CAN intersect the colliderLine
                if (!LinesCanIntersect(rayAsLine, colliderLine))
                {
                    continue;
                }

                //if it does, find where
                PointF intersectionPoint = LineIntersectionPoint(rayAsLine, colliderLine);
                //reject it if not in bounds,
                if (!IntersectionIsWithinLineSegments(intersectionPoint, origin, endPoint, b1, b2))
                {
                    continue;
                }
                //otherwise, add it to our found intersection points
                foundIntersectionPoints.Add(intersectionPoint);
                foundIntersectionPointNormals.Add(colliderLine.GetNormal().GetNormalAsDirection(intersectionPoint, origin));
            }

            //add the closest found intersection point to intersection points
            if(foundIntersectionPoints.Count == 0)
            {
                continue;
            }
            int closestPointIndex = 0;
            float closestPointDistance = float.PositiveInfinity;
            for(int i = 0; i < foundIntersectionPoints.Count; i++)
            {
                PointF p1 = origin;
                PointF p2 = foundIntersectionPoints[i];
                float dist = new Vector2(p2.X - p1.X, p2.Y - p1.Y).Length();
                if (dist < closestPointDistance)
                {
                    closestPointDistance = dist;
                    closestPointIndex = i;
                }
            }
            intersectionPoints.Add(foundIntersectionPoints[closestPointIndex]);
            hitColliders.Add(polyCol);
            normals.Add(foundIntersectionPointNormals[closestPointIndex]);

        }
        //for each circle
        //does it intersect the circle (twice)? if so, where
        //take closest intersection point
        foreach (Collider obj in verletObjects.OfType<CircleCollider>())
        {
            CircleCollider circCol = (CircleCollider)obj;
            float radius = circCol.GetRadius();
            Vector2 circPos = circCol.GetGlobalColliderPosition();
            PointF centre = new PointF(circPos.X, circPos.Y);
            if(!LineCanIntersectCircle(origin, endPoint, centre, radius)){ continue; }

            //get the two intersection points
            List<PointF> intersections = LineIntersectsCircle(origin, endPoint, centre, radius);
            float distToP1 = new Vector2(intersections[0].X - origin.X, intersections[0].Y - origin.Y).Length();
            float distToP2 = new Vector2(intersections[1].X - origin.X, intersections[1].Y - origin.Y).Length();

            if(distToP1 < distToP2)
            {
                intersectionPoints.Add(intersections[0]);
                Vector2 normal = new Vector2(intersections[0].X - centre.X, intersections[0].Y - centre.Y);
                if(normal != Vector2.Zero) { normal.Normalize(); }
                normals.Add(normal);
            }
            else
            {
                intersectionPoints.Add(intersections[1]);
                Vector2 normal = new Vector2(intersections[1].X - centre.X, intersections[1].Y - centre.Y);
                if (normal != Vector2.Zero) { normal.Normalize(); }
                normals.Add(normal);
            }

            hitColliders.Add(circCol);

        }


        if (intersectionPoints.Count == 0)
        {
            return new Raycast2DResult(null, new PointF(float.NaN, float.NaN), new Vector2(float.NaN, float.NaN));
        }

        //finally, choose the intersection point closest to the raycast origin
        int closestPointIndex2 = 0;
        float closestPointDistance2 = float.PositiveInfinity;
        for (int i = 0; i < intersectionPoints.Count; i++)
        {
            PointF p1 = origin;
            PointF p2 = intersectionPoints[i];
            float dist = new Vector2(p2.X - p1.X, p2.Y - p1.Y).Length();
            if (dist < closestPointDistance2)
            {
                closestPointDistance2 = dist;
                closestPointIndex2 = i;
            }
        }
        return new Raycast2DResult(
            hitColliders[closestPointIndex2],
            intersectionPoints[closestPointIndex2],
            normals[closestPointIndex2] );
    }
}

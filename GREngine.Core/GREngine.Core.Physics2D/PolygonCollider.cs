using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Microsoft.Xna.Framework.Color;
using GREngine.Core.Physics2D;
using GREngine.Core.System;
using GREngine.Core.PebbleRenderer;
using GREngine.Debug;
using System.Net.NetworkInformation;

namespace GREngine.Core.Physics2D;

public class PolygonCollider : Collider
{
    /// <summary>
    /// First points connects to second, last point connects to first
    /// Relative to position
    /// </summary>
    public List<PointF> vertices;

    public PolygonCollider(List<PointF> vertices) : base()
    {
        this.vertices = vertices;
    }

    public PolygonCollider(List<PointF> vertices, bool debugged) : base(debug: debugged)
    {
        this.vertices = vertices;
    }

    public PolygonCollider(List<PointF> vertices, string collisionLayer, bool debugged = false) :
        base(layer: collisionLayer, debug: debugged)
    {
        this.vertices = vertices;
    }
    public PolygonCollider(List<PointF> vertices, Vector2 offset, string collisionLayer, bool debugged = false) :
        base(layer: collisionLayer, debug: debugged, offset: offset)
    {
        this.vertices = vertices;
    }
    public PolygonCollider(List<PointF> vertices, Vector2 offset, bool debugged = false) :
        base(debug: debugged, offset: offset)
    {
        this.vertices = vertices;
    }

    private List<PointF> rotateVertices(List<PointF> vertices)
    {
        List<PointF> result = new List<PointF>();
        float rotationAngle = GetRotation() * (MathF.PI / 180.0f); //convert angle to cringe radians
        foreach (PointF relativePoint in vertices)
        {
            // Rotate the relative point clockwise
            float rotatedX =
                (relativePoint.X * MathF.Cos(rotationAngle)) - (relativePoint.Y * MathF.Sin(rotationAngle));
            float rotatedY =
                (relativePoint.X * MathF.Sin(rotationAngle)) + (relativePoint.Y * MathF.Cos(rotationAngle));

            result.Add(new PointF(rotatedX, rotatedY));
        }

        return result;
    }

    private List<PointF> translateVertices(List<PointF> vertices)
    {
        List<PointF> result = new List<PointF>();
        PointF origin = new PointF(GetGlobalColliderPosition().X, GetGlobalColliderPosition().Y);
        foreach (PointF relativePoint in vertices)
        {
            // Translate points to be relative to shape centre in worldspace
            result.Add(new PointF(origin.X + relativePoint.X, origin.Y + relativePoint.Y));
        }

        return result;
    }

    /// <summary>
    /// Looks through all transformed vertices and finds the
    /// minimum and maximum bounding points
    /// </summary>
    public override void CalculateAABB()
    {
        List<PointF> vertices = translateVertices(rotateVertices(this.vertices));
        PointF min = vertices[0];
        PointF max = vertices[1];
        foreach (PointF v in vertices)
        {
            min = new PointF((Math.Min(min.X, v.X)), Math.Min(min.Y, v.Y));
            max = new PointF((Math.Max(max.X, v.X)), Math.Max(max.Y, v.Y));
        }

        SetAABB(new AABB(min, max));
    }


    /// <summary>
    /// First checks if AABBs are overlapping before proceeding
    /// Then will do some stuff and resolve the collision
    /// </summary>
    /// <param name="other"></param>
    public override void SolveCollision(PolygonCollider other, Vector2 velocity)
    {
        //cache successful collision vectors
        List<Vector2> collisionVectors = new List<Vector2>();

        //calculate overlap region AABB
        AABB overlapRegion = collisionSystem.GetAABBOverlapRegion(GetAABB(), other.GetAABB());

        //if overlapregion size is like (0,x), (0,0), (x,0) then its not a collision
        if (overlapRegion.size().X == 0 || overlapRegion.size().Y == 0)
        {
            return;
        }

        //get motion and reverse motion vectors
        Vector2 velocityVector = velocity;
        if (velocityVector != Vector2.Zero)
        {
            velocityVector.Normalize();
        }
        AABB combinedAABB = collisionSystem.GetCombinedAABBRegion(GetAABB(), other.GetAABB());
        float vectorScale = new Vector2(
            combinedAABB.max.X - combinedAABB.min.X, combinedAABB.max.Y - combinedAABB.min.Y).Length();
        Vector2 forwardVector = velocityVector * vectorScale;
        Vector2 reverseVector = new Vector2(forwardVector.X * -1, forwardVector.Y * -1);

        //for all of our vertices:
        List<PointF> vertices = translateVertices(rotateVertices(this.vertices));
        List<PointF> otherVertices = other.translateVertices(other.rotateVertices(other.vertices));

        Vector2 position = GetGlobalColliderPosition();
        Vector2 otherPosition = other.GetGlobalColliderPosition();
        bool velocityTowardsCollision = false;
        float distToOtherObj = new Vector2(otherPosition.X - position.X, otherPosition.Y - position.Y).Length();
        Vector2 posAfterMove = position + velocityVector;
        float distAfterMove = new Vector2(
            otherPosition.X - posAfterMove.X, otherPosition.Y - posAfterMove.Y).Length();
        if (distAfterMove < distToOtherObj)
        {
            velocityTowardsCollision = true;
        }

        foreach (PointF v in vertices)
        {
            //check if point is in AABB overlap region
            if (!collisionSystem.PointIsInAABB(v, overlapRegion))
            {
                continue;
            }

            //apply reverse motion vector
            Vector2 directionVector = reverseVector;
            if (!velocityTowardsCollision) { directionVector = forwardVector; }
            PointF a2 = new PointF(v.X + directionVector.X, v.Y + directionVector.Y);

            List<Line> otherColliderLines = new List<Line>();
            for (int i = 0; i < otherVertices.Count; i++)
            {
                PointF b1 = otherVertices[i];
                PointF b2 = i == otherVertices.Count - 1 ? otherVertices[0] : otherVertices[i + 1];
                otherColliderLines.Add(new Line(b1, b2));
            }

            //for all lines in other collider:
            for (int i = 0; i < otherVertices.Count; i++)
            {
                PointF b1 = otherVertices[i];
                PointF b2 = i == otherVertices.Count - 1 ? otherVertices[0] : otherVertices[i + 1];
                //check if this line equation intersects other collider line equation
                //first check AABB overlap, then ensure they arent parallel
                if (!collisionSystem.LinesSegmentsOverlap(v, a2, b1, b2))
                {
                    continue;
                }
                Line l1 = new Line(v, a2);
                Line l2 = new Line(b1, b2);
                if (!collisionSystem.LinesCanIntersect(l1, l2))
                {
                    continue;
                }

                //if they can intersect, find where
                PointF intersectionPoint = collisionSystem.LineIntersectionPoint(l1, l2);
                //reject it if not in bounds,
                if (!collisionSystem.IntersectionIsWithinLineSegments(intersectionPoint, v, a2, b1, b2))
                {
                    continue;
                }
                //otherwise, find vector from vertex to intersection point
                // and add it to list of possible motion vectors
                Vector2 collisionVector = new Vector2(v.X - intersectionPoint.X, v.Y - intersectionPoint.Y);

                if (!velocityTowardsCollision)
                {
                    collisionVector = new Vector2(intersectionPoint.X - v.X, intersectionPoint.Y - v.Y);
                }
                collisionVectors.Add(collisionVector);
            }
        }


        //for all of other colliders vertices:
        foreach (PointF v in otherVertices)
        {
            //check if point is in AABB overlap region
            if (!collisionSystem.PointIsInAABB(v, overlapRegion))
            {
                continue;
            }

            //apply forward motion vector
            Vector2 directionVector = forwardVector;
            if (!velocityTowardsCollision) { directionVector = reverseVector; }
            PointF a2 = new PointF(v.X + directionVector.X, v.Y + directionVector.Y);

            List<Line> ourColliderLines = new List<Line>();
            for (int i = 0; i < otherVertices.Count; i++)
            {
                PointF b1 = otherVertices[i];
                PointF b2 = i == otherVertices.Count - 1 ? otherVertices[0] : otherVertices[i + 1];
                ourColliderLines.Add(new Line(b1, b2));
            }

            //for all lines in our collider:
            for (int i = 0; i < vertices.Count; i++)
            {
                PointF b1 = vertices[i];
                PointF b2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];
                //check if this line equation intersects our collider line equation
                if (!collisionSystem.LinesSegmentsOverlap(v, a2, b1, b2))
                {
                    continue;
                }
                Line l1 = new Line(v, a2);
                Line l2 = new Line(b1, b2);
                if (!collisionSystem.LinesCanIntersect(l1, l2))
                {
                    continue;
                }
                //if it does, find where
                PointF intersectionPoint = collisionSystem.LineIntersectionPoint(l1, l2);
                //reject it if not in bounds,
                if (!collisionSystem.IntersectionIsWithinLineSegments(intersectionPoint, v, a2, b1, b2))
                {
                    continue;
                }

                //otherwise, find vector from vertex to intersection point
                // and add it to list of possible motion vectors
                Vector2 collisionVector = new Vector2(intersectionPoint.X - v.X, intersectionPoint.Y - v.Y);

                if (!velocityTowardsCollision)
                {
                    collisionVector = new Vector2(v.X - intersectionPoint.X, v.Y - intersectionPoint.Y);
                }
                collisionVectors.Add(collisionVector);

            }
        }

        //finally, resolve by moving in vector of greatest magnitude.
        Vector2 vectorWithGreatestMagnitude = new Vector2(0, 0);
        if (collisionVectors.Count > 0)
        {
            vectorWithGreatestMagnitude = collisionVectors[0];

            for (int i = 1; i < collisionVectors.Count; i++)
            {
                if (collisionVectors[i].LengthSquared() > vectorWithGreatestMagnitude.LengthSquared())
                {
                    vectorWithGreatestMagnitude = collisionVectors[i];
                }
            }

            ResolveCollision(other, vectorWithGreatestMagnitude);
        }
        else { return; }
    }
    public override void SolveCollision(CircleCollider other, Vector2 velocity)
    {
        AABB overlapRegion = collisionSystem.GetAABBOverlapRegion(GetAABB(), other.GetAABB());
        //cache successful collision vectors
        List<Vector2> collisionVectors = new List<Vector2>();
        // get motion and reverse motion vectors
        Vector2 velocityVector = velocity;
        if (velocityVector != Vector2.Zero)
        {
            velocityVector.Normalize();
        }
        AABB combinedAABB = collisionSystem.GetCombinedAABBRegion(GetAABB(), other.GetAABB());
        float vectorScale = new Vector2(
            combinedAABB.max.X - combinedAABB.min.X, combinedAABB.max.Y - combinedAABB.min.Y).Length();
        Vector2 forwardVector = velocityVector * vectorScale;
        Vector2 reverseVector = new Vector2(forwardVector.X * -1, forwardVector.Y * -1);

        Vector2 position = GetGlobalColliderPosition();
        Vector2 otherPosition = other.GetGlobalColliderPosition();
        bool velocityTowardsCollision = false;
        float distToOtherObj = new Vector2(otherPosition.X - position.X, otherPosition.Y - position.Y).Length();
        Vector2 posAfterMove = position + velocityVector;
        float distAfterMove = new Vector2(
            otherPosition.X - posAfterMove.X, otherPosition.Y - posAfterMove.Y).Length();
        if (distAfterMove < distToOtherObj)
        {
            velocityTowardsCollision = true;
        }

        //get all of our collider vertices that are inside the colliding circle
        List<PointF> vertices = translateVertices(rotateVertices(this.vertices));
        List<PointF> vertsInCircle = new List<PointF>();
        foreach (PointF vertex in vertices)
        {
            if (other.PointInsideCollider(vertex)) { vertsInCircle.Add(vertex); }
        }

        //for each vertex that IS inside the circle, we make a line along the motion vector
        //check where that line intersect the circle
        //take the intersection point thats in the AABB overlap region
        //collision vector is then the vertex to the intersection point
        Vector2 directionVector = reverseVector;
        if (!velocityTowardsCollision) { directionVector = forwardVector; }

        PointF circCenter = new PointF(otherPosition.X, otherPosition.Y);
        float circRadius = other.GetRadius();
        foreach (PointF vertex in vertsInCircle)
        {
            PointF p1 = new PointF(vertex.X + directionVector.X, vertex.Y + directionVector.Y);
            PointF p2 = new PointF(vertex.X + (directionVector.X * -1), vertex.Y + (directionVector.Y * -1));
            List<PointF> intersections = collisionSystem.LineIntersectsCircle(p1, p2, circCenter, circRadius);

            if (collisionSystem.PointIsInAABB(intersections[0], overlapRegion))
            {
                Vector2 collisionVector = new Vector2(vertex.X - intersections[0].X, vertex.Y - intersections[0].Y);

                if (!velocityTowardsCollision)
                {
                    collisionVector = new Vector2(intersections[0].X - vertex.X, intersections[0].Y - vertex.Y);
                }
                collisionVectors.Add(collisionVector);
            }
            else
            {
                Vector2 collisionVector = new Vector2(
                    vertex.X - intersections[1].X, vertex.Y - intersections[1].Y);

                if (!velocityTowardsCollision)
                {
                    collisionVector = new Vector2(intersections[1].X - vertex.X, intersections[1].Y - vertex.Y);
                }
                collisionVectors.Add(collisionVector);
            }
        }

        //if we have some collision vector at this point, we choose the largest one
        Vector2 vectorWithGreatestMagnitude = new Vector2(0, 0);
        if (collisionVectors.Count > 0)
        {
            vectorWithGreatestMagnitude = collisionVectors[0];

            for (int i = 1; i < collisionVectors.Count; i++)
            {
                if (collisionVectors[i].LengthSquared() > vectorWithGreatestMagnitude.LengthSquared())
                {
                    vectorWithGreatestMagnitude = collisionVectors[i];
                }
            }

            ResolveCollision(other, vectorWithGreatestMagnitude);
            return;
        }

        //otherwise, we get all the lines of our collider, and cast a line from
        // the centre of the circle, and along the normal to the line, and find the intersection point
        //for all lines in our collider:
        for (int i = 0; i < vertices.Count; i++)
        {
            PointF b1 = vertices[i];
            PointF b2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];
            Line l2 = new Line(b1, b2);

            //make l1 a line from the circle centre, in direction normal towards l2
            Line l1 = l2.GetNormal();

            //vertical line
            if (l1.a == 0) { l1 = new Line(0, -(circCenter.X), 1); }
            else if (l1.m == 0) { l1 = new Line(1, circCenter.Y, 1); }
            else { l1 = new Line(1, circCenter.Y - (l1.m * circCenter.X), l1.m); }

            if (!collisionSystem.LinesCanIntersect(l1, l2))
            {
                continue;
            }
            //if it can intersect, find where
            PointF intersectionPoint = collisionSystem.LineIntersectionPoint(l1, l2);
            if (!collisionSystem.PointIsInAABB(intersectionPoint, overlapRegion))
            {
                continue;
            }

            Vector2 centreToIntersection = new Vector2(
                intersectionPoint.X - circCenter.X, intersectionPoint.Y - circCenter.Y);
            if (!velocityTowardsCollision)
            {
                centreToIntersection = new Vector2(
                circCenter.X - intersectionPoint.X,
                circCenter.Y - intersectionPoint.Y);
            }
            if (centreToIntersection == Vector2.Zero) { continue; }
            Vector2 centreToIntersectionDir = centreToIntersection;
            if (centreToIntersectionDir.Length() == 0) centreToIntersectionDir.Normalize();
            Vector2 centreToRadiusEdge = centreToIntersectionDir * circRadius;
            if (centreToIntersection.Length() < centreToRadiusEdge.Length())
            {
                Vector2 collisionVector = centreToIntersection - centreToRadiusEdge;
                collisionVectors.Add(collisionVector);
            }

        }

        //then we choose the vector of smallest magnitude
        Vector2 vectorWithSmallestMagnitude = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        if (collisionVectors.Count > 0)
        {
            vectorWithSmallestMagnitude = collisionVectors[0];

            for (int i = 1; i < collisionVectors.Count; i++)
            {
                if (collisionVectors[i].LengthSquared() < vectorWithGreatestMagnitude.LengthSquared())
                {
                    vectorWithSmallestMagnitude = collisionVectors[i];
                }
            }

            ResolveCollision(other, vectorWithSmallestMagnitude);
        }
    }

    /// <summary>
    /// This is where we actually resolve the collision, as in move stuff
    /// Here we decide what we want to move and how, since other collider could be static
    /// </summary>
    /// <param name="other"></param>
    private void ResolveCollision(PolygonCollider other, Vector2 collisionVector)
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
            Vector2 resolvedPosition = new Vector2(
                currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
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

            Vector2 resolvedPosition = new Vector2(
                currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
            Vector2 otherColliderResolvedPosition = new Vector2(
                otherCurrentPos.X + otherColliderResolution.X,
                otherCurrentPos.Y + otherColliderResolution.Y);
            SetNodePosition(resolvedPosition);
            other.SetNodePosition(otherColliderResolvedPosition);
        }
    }

    private void ResolveCollision(CircleCollider circCollider, Vector2 collisionVector)
    {
        FireCorrectEvent(collisionVector, circCollider);

        Vector2 currentPos = GetGlobalColliderPosition();
        Vector2 otherCurrentPos = circCollider.GetGlobalColliderPosition();

        Vector2 thisColliderResolution = new Vector2(collisionVector.X * -1, collisionVector.Y * -1);
        if (IsTrigger() || circCollider.IsTrigger())
        {
            return;
        }
        //normal to static, resolve only normal
        else if (circCollider.IsStatic())
        {
            Vector2 resolvedPosition = new Vector2(
                currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
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

            Vector2 resolvedPosition = new Vector2(
                currentPos.X + thisColliderResolution.X, currentPos.Y + thisColliderResolution.Y);
            Vector2 otherColliderResolvedPosition = new Vector2(
                otherCurrentPos.X + otherColliderResolution.X,
                otherCurrentPos.Y + otherColliderResolution.Y);
            SetNodePosition(resolvedPosition);
            circCollider.SetNodePosition(otherColliderResolvedPosition);
        }
    }

    public override bool PointInsideCollider(PointF point)
    {

        CalculateAABB();
        AABB colliderAABB = GetAABB();
        float aabbWidth = colliderAABB.max.X - colliderAABB.min.X;
        float aabbHeight = colliderAABB.max.Y - colliderAABB.min.Y;

        //If point is not the AABB, it most certainly isnt in the collider
        if (!collisionSystem.PointIsInAABB(point, colliderAABB))
        {
            return false;
        }

        List<Vector2> vectors = new List<Vector2> {
            new Vector2(0, aabbHeight),
            new Vector2(0, -aabbHeight),
            new Vector2(aabbWidth, 0),
            new Vector2(-aabbWidth, 0),
        };
        List<PointF> vertices = translateVertices(rotateVertices(this.vertices));

        int hitCount = 0;
        //for all lines in our collider:
        foreach (Vector2 v in vectors)
        {
            PointF a2 = new PointF(point.X + v.X, point.Y + v.Y);
            for (int i = 0; i < vertices.Count; i++)
            {
                PointF b1 = vertices[i];
                PointF b2 = i == vertices.Count - 1 ? vertices[0] : vertices[i + 1];
                //check if this line equation intersects our collider line equation
                if (!collisionSystem.LinesSegmentsOverlap(point, a2, b1, b2))
                {
                    continue;
                }
                Line l1 = new Line(point, a2);
                Line l2 = new Line(b1, b2);
                if (!collisionSystem.LinesCanIntersect(l1, l2))
                {
                    continue;
                }
                //if it does, find where
                PointF intersectionPoint = collisionSystem.LineIntersectionPoint(l1, l2);
                //reject it if not in bounds,
                if (!collisionSystem.IntersectionIsWithinLineSegments(intersectionPoint, point, a2, b1, b2))
                {
                    continue;
                }

                //otherwise, this point + this direction has intersected the shape somewhere
                hitCount++;
                break;
            }
        }
        return hitCount == 4;

    }
    /// <summary>
    /// Get all vertices of this polygon collider,
    /// translated and rotated to the correct world position
    /// </summary>
    /// <returns></returns>
    public List<PointF> GetVertices()
    {
        return translateVertices(rotateVertices(this.vertices));
    }

    /// <summary>
    /// Draws the Polygon onto the provided SpriteBatch
    /// </summary>
    public override void DrawDebug()
    {
        Color lineColor = new Color(16, 245, 0); // Light green color
        float lineThickness = 2f;

        List<PointF> verticesDebug = translateVertices(rotateVertices(this.vertices));

        // Ensure there are enough vertices to draw lines
        if (verticesDebug.Count < 2)
        {
            return;
        }

        // Draw lines connecting vertices
        for (int i = 0; i < verticesDebug.Count - 1; i++)
        {
            Vector2 start = verticesDebug[i].ToVector2();
            Vector2 end = verticesDebug[i + 1].ToVector2();
            DrawLine(start, end, lineColor, lineThickness);
        }

        // Connect the last vertex to the first one
        Vector2 last = verticesDebug[verticesDebug.Count - 1].ToVector2();
        Vector2 first = verticesDebug[0].ToVector2();
        DrawLine(last, first, lineColor, lineThickness);


        CalculateAABB();
        //Draw AABB
        lineColor = new Color(69, 243, 255); // Light blue color
        if (IsAABBOverlapping())
        {
            lineColor = new Color(252, 3, 236); //Light pink color
        }
        lineThickness = 2f;
        AABB aabb = GetAABB();
        Vector2 tl = new Vector2(aabb.min.X, aabb.min.Y);
        Vector2 tr = new Vector2(aabb.max.X, aabb.min.Y);
        Vector2 br = new Vector2(aabb.max.X, aabb.max.Y);
        Vector2 bl = new Vector2(aabb.min.X, aabb.max.Y);
        /*DrawLine(spriteBatch, pixelTexture, tl, tr, lineColor, lineThickness);
        DrawLine(spriteBatch, pixelTexture, tr, br, lineColor, lineThickness);
        DrawLine(spriteBatch, pixelTexture, br, bl, lineColor, lineThickness);
        DrawLine(spriteBatch, pixelTexture, bl, tl, lineColor, lineThickness);*/

        SetAABBOverlapping(false);
    }


    private void DrawLine(Vector2 start, Vector2 end, Color color, float lineThickness)
    {
        //Vector2 edge = end - start;
        //float angle = (float)Math.Atan2(edge.Y, edge.X);

        //// Draw a 1-pixel wide rectangle for the line
        //spriteBatch.Draw(pixelTexture, start, null, color,
        //          angle, Vector2.Zero, new Vector2(edge.Length(), lineThickness), SpriteEffects.None, 0);
        Game.Services.GetService<IPebbleRendererService>().drawDebug(
            new DebugDrawable(start, end, color, DebugShape.LINE));
    }
}

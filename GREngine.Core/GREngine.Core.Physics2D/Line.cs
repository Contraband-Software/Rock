using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net;

namespace GREngine.Core.Physics2D;

/// <summary>
/// Form: Ay = mx + c
/// Standard A = 1, m = gradient
/// Becomes A = 0, m = 1, c = -(x intercept) when line is vertical
/// </summary>
public struct Line
{
    public float a;
    public float c;
    public float m;

    public Line(float c, float m)
    {
        this.a = 1;
        this.c = c;
        this.m = m;
    }
    public Line(float a, float c, float m)
    {
        this.a = a;
        this.c = c;
        this.m = m;
    }

    public Line(PointF p1, PointF p2)
    {
        // Step 2: Calculate slopes and intercepts
        float a, c, m;

        if (MathF.Abs(p2.X - p1.X) < float.Epsilon)
        {
            // Line is vertical
            a = 0;
            m = 1;
            c = -(p1.X);
        }
        else
        {
            a = 1;
            m = (p2.Y - p1.Y) / (p2.X - p1.X);
            c = p1.Y - m * p1.X;
        }

        this.a = a;
        this.c = c;
        this.m = m;
    }

    /// <summary>
    /// Returns a line equation that is normal to this one
    /// </summary>
    /// <returns></returns>
    public Line GetNormal()
    {
        //normal to vertical line
        if(this.a == 0)
        {
            float m = 0f;
            float c = 1;
            return new Line(c, m);
        }
        //normal to horizontal line
        else if(this.m == 0)
        {
            float a = 0f;
            float c = -1;
            float m = 1f;
            return new Line(a, c, m);
        }
        //normal to a standard line
        else
        {
            float m = (-1f / this.m);
            return new Line(this.c, m);
        }
    }

    private bool PointIsOnLine(PointF point)
    {
        //line is vertical
        if (this.a == 0)
        {
            return (-point.X) == c;
        }
        //line is horizontal
        else if (this.m == 0)
        {
            return (point.Y) == c;
        }
        //standard line
        else
        {
            return ((this.a * point.Y) - (this.m * point.X) - c == 0); 
        }
    }

    /// <summary>
    /// Returns a directional vector of this line, assuming its a normal
    /// </summary>
    /// <param name="towardsPoint">reference point towards which to extend</param>
    /// <returns></returns>
    public Vector2 GetNormalAsDirection(PointF fromPoint, PointF towardsPoint)
    {
        //translate line so that it goes through the fromPoint
        Line line;
        if (this.a == 0) { line = new Line(0, -(fromPoint.X), 1); }
        else if (this.m == 0) { line = new Line(1, fromPoint.Y, 1); }
        else { line = new Line(1, fromPoint.Y - (this.m * fromPoint.X), this.m); }

        if (!line.PointIsOnLine(fromPoint))
        {
            throw new InvalidOperationException("fromPoint is not on the line");
        }
        //line is vertical
        if (line.a == 0)
        {
            //use Y point
            Vector2 direction = new Vector2(0f, towardsPoint.Y - fromPoint.Y);
            if(direction != Vector2.Zero) { direction.Normalize(); }
            return direction;
        }
        //line is horizontal
        else if (line.m == 0)
        {
            //use X point
            Vector2 direction = new Vector2(towardsPoint.X - fromPoint.X, 0f);
            if (direction != Vector2.Zero) { direction.Normalize(); }
            return direction;
        }
        //standard line
        else
        {
            //use X of towardsPoint, find Y on line
            float endY = (line.m * towardsPoint.X) + line.c;
            Vector2 incoming = new Vector2(fromPoint.X - towardsPoint.X, fromPoint.Y - towardsPoint.Y);
            Vector2 normal = new Vector2(towardsPoint.X - fromPoint.X, endY - fromPoint.Y);

            //dot product: fromPoint - towardsPoint, endPoint - fromPoint
            //if dot product is positive, flip the normal
            if(Vector2.Dot(incoming, normal) > 0f)
            {
                normal *= -1f;
            }
            if (normal != Vector2.Zero) { normal.Normalize(); }
            return normal;
            
        }
    }
}

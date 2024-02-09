using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
}

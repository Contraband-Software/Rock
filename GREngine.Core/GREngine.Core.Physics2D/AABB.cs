using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace GREngine.Core.Physics2D;
public struct AABB
{
    public PointF min;
    public PointF max;
    public AABB(PointF min, PointF max)
    {
        this.min = min;
        this.max = max;
    }

    public Vector2 size()
    {
        return new Vector2(max.X - min.X, max.Y - min.Y);
    }
}

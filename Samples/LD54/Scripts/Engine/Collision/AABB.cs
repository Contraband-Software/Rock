namespace LD54.Engine.Collision;

using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;


public struct AABB
{
    public Vector3 min { get; set; }
    public Vector3 max { get; set; }

    public AABB(Vector3 min, Vector3 max)
    {
        this.min = min;
        this.max = max;
    }
}


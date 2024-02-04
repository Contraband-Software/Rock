namespace LD54.Engine.Collision;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


public struct Overlap
{
    public bool isOverlap = false;
    public float[] overlaps = new float[4] { 0, 0, 0, 0 };

    //for circles
    public float overlapDistance = 0f;
    public Vector2 posDelta = Vector2.Zero;

    public Overlap(bool isOverlap)
    {
        this.isOverlap = isOverlap;
    }

    public Overlap(bool isOverlap, float[] overlaps)
    {
        this.isOverlap = isOverlap;
        this.overlaps = overlaps;
    }

    public Overlap(bool isOverlap, Vector2 posDelta, float overlapDistance)
    {
        this.isOverlap= isOverlap;
        this.posDelta = posDelta;
        this.overlapDistance = overlapDistance;
    }
}


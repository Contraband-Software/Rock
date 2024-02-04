namespace LD54.Engine.Collision;

using System;
using System.Collections.Generic;

public class Collision
{
    public Overlap overlap;
    public ColliderComponent collider;
    public Collision(ColliderComponent other, Overlap overlap)
    {
        this.overlap = overlap;
        this.collider = other;
    }
}

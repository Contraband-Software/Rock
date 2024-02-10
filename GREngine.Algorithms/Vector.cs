namespace GREngine.Algorithms;

using System;
using Microsoft.Xna.Framework;

public static class Vector
{
    public static Vector2 AngleToVector(float angle)
    {
        return new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
    }

    public static float VectorToAngle(Vector2 normalizedVector)
    {
        return (float)Math.Atan2(normalizedVector.X, -normalizedVector.Y);
    }
}

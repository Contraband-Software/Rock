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

    public static Vector2 SafeNormalize(Vector2 vec)
    {
        if (vec.Length() == 0)
            return Vector2.Zero;
        Vector2 v2 = new Vector2(vec.X, vec.Y);
        v2.Normalize();
        return v2;
    }
}

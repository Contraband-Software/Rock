namespace LD54.Engine;

using System;
using Microsoft.Xna.Framework;

public static class Math
{
    public static bool IsFinite(Vector2 vector2)
    {
        return float.IsFinite(vector2.X) && float.IsFinite(vector2.Y);
    }

    public static bool IsFinite(Vector3 vector3)
    {
        return float.IsFinite(vector3.X) && float.IsFinite(vector3.Y) && float.IsFinite(vector3.Z);
    }

    public static Vector2 RNormalize(this Vector2 vector2)
    {
        vector2.Normalize();
        return vector2;
    }

    public static Vector3 RNormalize(this Vector3 vector3)
    {
        vector3.Normalize();
        return vector3;
    }

    public static Vector2 Copy(this Vector2 vector2)
    {
        return new Vector2(vector2.X, vector2.Y);
    }

    public static Vector3 Copy(this Vector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.Y, -vector2.X);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.Y, vector2.X);
    }

    public static Vector2 Abs(this Vector2 vector2)
    {
        return new Vector2(MathF.Abs(vector2.X), MathF.Abs(vector2.Y));
    }

    public static Vector2 Clamp(this Vector2 vector2, float clampMin, float clampMax)
    {
        return new Vector2(System.Math.Clamp(vector2.X, clampMin, clampMax), System.Math.Clamp(vector2.Y, clampMin, clampMax));
    }

    public static float Magnitude(this Vector2 vector2)
    {
        return MathF.Sqrt(MathF.Pow(vector2.X, 2) + MathF.Pow(vector2.Y, 2));
    }

    public static Vector2 SwizzleXY(this Vector3 vector3)
    {
        return new Vector2(vector3.X, vector3.Y);
    }
}

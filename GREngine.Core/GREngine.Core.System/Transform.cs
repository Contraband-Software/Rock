namespace GREngine.Core.System;

using Microsoft.Xna.Framework;

public struct Transform
{
    public Matrix matrix = Matrix.Identity;

    public Transform(Matrix m)
    {
        this.matrix = m;
    }
}

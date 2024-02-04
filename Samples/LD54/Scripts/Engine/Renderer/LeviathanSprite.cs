namespace LD54.Engine.Leviathan;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LeviathanSprite
{
    public Texture2D color;
    public Texture2D? normal;
    private Matrix transform;
    public Vector2 size;
    public bool useNormal = false;
    public bool isOccluder;
    private Game game;
    public int shader = 0;
    public float rotation = 0;
    private float layer = 0;
    public bool isLit = true;

    public LeviathanSprite(Game game, Matrix transform, float rotation, Vector2 size, Texture2D colorTexture, Texture2D? normalTexture = null, bool isOccluder = true, float layer = 0, bool isLit = true)
    {
        this.color = colorTexture;
        this.normal = normalTexture;
        this.useNormal = normalTexture != null;
        this.transform = transform;
        this.game = game;
        this.size = size;
        this.isOccluder = isOccluder;
        this.rotation = rotation;
        this.layer = layer;
        this.isLit = isLit;
    }
    public LeviathanSprite(Game game, Matrix transform,float rotation, Vector2 size, int shader, Texture2D colorTexture, bool isOccluder = true, float layer = 0, bool isLit = true)
    {
        this.color = colorTexture;
        this.transform = transform;
        this.game = game;
        this.size = size;
        this.isOccluder = isOccluder;
        this.shader = shader;
        this.rotation = rotation;
        this.layer = layer;
        this.isLit = isLit;
    }
    public LeviathanSprite(Game game, Matrix transform, float rotation, Vector2 size, int shader, Texture2D colorTexture, Texture2D? normalTexture = null, bool isOccluder = true, float layer = 0, bool isLit = true)
    {
        this.color = colorTexture;
        this.normal = normalTexture;
        this.useNormal = normalTexture != null;
        this.transform = transform;
        this.game = game;
        this.size = size;
        this.isOccluder = isOccluder;
        this.shader = shader;
        this.rotation = rotation;
        this.layer = layer;
        this.isLit = isLit;
    }

    public void SetColorTexture(Texture2D color)
    {
        this.color = color;
    }

    public void SetTransform(Matrix transform)
    {
        this.transform = transform;
    }

    public Vector3 GetPosition()
    {
        return this.transform.Translation;
    }

    public Vector2 GetPositionXY()
    {
        return new Vector2(transform.Translation.X, transform.Translation.Y);
    }
    public Vector2 GetPositionXYCenter()
    {
        return new Vector2(transform.Translation.X + color.Width*size.X/2, transform.Translation.Y + color.Height * size.Y/2);
    }


    public void SetPosition(Vector3 position) {
        this.transform.Translation = position;
    }

    public void TranslatePosition(Vector3 translation)
    {
        this.transform.Translation += translation;
    }
    public float getDepth()
    {
        return layer;
    }
}

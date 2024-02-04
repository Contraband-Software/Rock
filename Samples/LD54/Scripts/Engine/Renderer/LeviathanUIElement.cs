namespace LD54.Engine.Leviathan;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class LeviathanUIElement
{
    public Texture2D color;
    private Matrix transform;
    public Vector2 size;
    private Game game;
    public bool isEnabled;
    public bool isText = false;
    public string text;
    public SpriteFont font;
    public Color textColor;

    public LeviathanUIElement(Game game, Matrix transform, Vector2 size, Texture2D colorTexture, bool isEnabled = true)
    {
        this.color = colorTexture;
        this.transform = transform;
        this.game = game;
        this.size = size;
        this.isEnabled = isEnabled;
    }

    public LeviathanUIElement(Game game, Matrix transform, Vector2 size, string text, SpriteFont font,Color color, bool isEnabled = true)
    {
        isText = true;
        this.text = text;
        this.font = font;
        this.transform = transform;
        this.game = game;
        this.size = size;
        this.isEnabled = isEnabled;
        this.textColor = color;
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
        return new Vector2(this.transform.Translation.X, this.transform.Translation.Y);
    }

    public void SetPosition(Vector3 position)
    {
        this.transform.Translation = position;
    }

    public void TranslatePosition(Vector3 translation)
    {
        this.transform.Translation += translation;
    }
}


namespace LD54.Engine.Components;

using Leviathan;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class SpriteRendererComponent : Component
{
    private LeviathanSprite? sprite;

    private ILeviathanEngineService re;

    public Vector3 Offset = Vector3.Zero;

    public float Rotation = 0;

    public SpriteRendererComponent(string name, Game appCtx) : base(name, appCtx)
    {

    }

    public void LoadSpriteData(Matrix transform, Vector2 size, Texture2D colorTexture, float depth = 0.5f, Texture2D? normalTexture = null, int shader = 0, bool isOccluder = true, bool isLit = true)
    {
        re = this.app.Services.GetService<ILeviathanEngineService>();
        sprite = new(this.app, transform, 0, size,shader, colorTexture, normalTexture, isOccluder, depth,isLit);
        re.addSprite(sprite);
    }

    public override void OnLoad(GameObject? parentObject)
    {
        gameObject = parentObject;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Matrix transform = gameObject.GetGlobalTransform();
        transform.Translation += this.Offset;
        sprite.SetTransform(transform);
        this.sprite.rotation = Rotation;
    }

    public override void OnUnload()
    {
        this.app.Services.GetService<ILeviathanEngineService>().removeSprite(sprite);
        // PrintLn("OnUnload: SpriteRendererComponent");
    }

    public void SetSprite(Texture2D texture)
    {
        if(sprite != null)
        {
            sprite.SetColorTexture(texture);
        }
    }
}

namespace LD54.AsteroidGame.GameObjects;

using Engine.Collision;
using Engine.Components;
using LD54.Engine.Leviathan;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class StaticSprite : GameObject
{
    private Texture2D? texture;
    private Vector2 position;
    private Vector2 size;
    private SpriteRendererComponent sr;
    private ILeviathanEngineService re;
    private LeviathanShader shader;
    private GameObject blackhole;
    private Texture2D normal;

    public StaticSprite(GameObject blackhole,LeviathanShader shader, Texture2D texture, Vector2 position, Vector2 size  ,string name, Game appCtx, Texture2D? normal = null) : base(name, appCtx)
    {
        this.texture = texture;
        this.normal = normal;
        this.position = position;
        this.size = size;
        this.shader = shader;
        this.blackhole = blackhole;
    }

    public override void OnLoad(GameObject? parentObject)
    {
        re = this.app.Services.GetService<ILeviathanEngineService>();
        sr = new SpriteRendererComponent("spriteRenderer",this.app);
        sr.LoadSpriteData(Matrix.CreateTranslation(position.X, position.Y, 0), this.size, texture, 0.0f, normal, 2,false,false);
        this.AddComponent(sr);
    }
    public override void Update(GameTime gameTime)

    {
        //PrintLn()
        Vector2 pos = -re.GetCameraPosition() + new Vector2(blackhole.GetGlobalPosition().X, blackhole.GetGlobalPosition().Y);
        this.shader.UpdateParam("blackholeX", pos.X);
        this.shader.UpdateParam("blackholeY", pos.Y);

        base.Update(gameTime);
        sr.Offset = new Vector3(re.GetCameraPosition().X-0, re.GetCameraPosition().Y-0, 0);
    }
}

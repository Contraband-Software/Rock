#nullable enable


using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

using GREngine.Core.System;

namespace GREngine.Core.PebbleRenderer;

public class Sprite : Behaviour
{
    private Texture2D[] textures;

    public Vector2 position;
    private Vector2 scale = new Vector2(1);

    public Point size;
    public float rotation = 0;
    private Vector2 offset; //I am planning for sprites to have center origin, maybe expose this later?

    public bool isShadowCaster;
    public bool isLit = true;

    public int material = 0;
    public int layer = 0;
    public Sprite(float rotation, Vector2 scale, Texture2D diffuse, Texture2D? normal, Texture2D? roughness,int layer = 0, int material = 0, bool isShadowCaster = true, bool isLit = true) {

        this.textures = new Texture2D[3] { diffuse, normal, roughness };
        this.position = new Vector2(0);
        this.rotation = rotation;
        this.material = material;
        this.layer = layer; //layer0 = behind everything
        this.isShadowCaster = isShadowCaster;
        this.isLit = isLit;
        this.scale = scale;
        calculateOffset();
        calculateSize();
    }

    protected override void OnStart()
    {
        Game.Services.GetService<IPebbleRendererService>().addSprite(this);
        //base.OnStart();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        this.position.X = this.Node.GetGlobalPosition().X;
        this.position.Y = this.Node.GetGlobalPosition().Y;
        //base.OnUpdate(gameTime);
    }

    private void calculateOffset()
    {
        offset = new Vector2(textures[0].Width / 2, textures[0].Height / 2);
    }

    private void calculateSize()
    {
        size.X = (int)Math.Floor(textures[0].Width * scale.X);
        size.Y = (int)Math.Floor(textures[0].Height * scale.Y);
    }


    public void setScale(Vector2 scale)
    {
        this.scale = scale;
        calculateSize();
    }

    public virtual void draw(SpriteBatch spriteBatch, int textureIndex)
    {
        if(textureIndex == 4) {
            spriteBatch.Draw(textures[0], new Rectangle(position.ToPoint(), size), null, Color.Black, rotation, offset, SpriteEffects.None, 0);//draw occluder mask
            return;
        }
        if (textures[textureIndex] != null)
        {
            spriteBatch.Draw(textures[textureIndex], new Rectangle(position.ToPoint(), size), null, Color.White, rotation, offset, SpriteEffects.None, 0);
        }
        else
        {
            spriteBatch.Draw(textures[0], new Rectangle(position.ToPoint(), size), null, Color.Black, rotation, offset, SpriteEffects.None, 0);//use diffuse as mask when no normal/diffuse texture provided
        }
    }

    protected override void OnDestroy()
    {
        Game.Services.GetService<IPebbleRendererService>().removeSprite(this);
    }



}


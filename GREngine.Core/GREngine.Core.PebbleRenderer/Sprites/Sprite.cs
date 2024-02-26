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

using Debug;

public class Sprite : Behaviour
{
    internal Texture2D[] textures;

    internal Vector2 position;
    public Vector2 offset;
    public Vector2 offsetToCenter;
    internal Vector2 scale = new Vector2(1);

    public Point size;
    public float rotation = 0;

    public bool isShadowCaster;
    public bool isLit = true;

    public int material = 0;
    public int layer = 0;
    public Sprite(Vector2 offset, float rotation, Vector2 scale, Texture2D diffuse, Texture2D? normal, Texture2D? roughness,int layer = 0, int material = 0, bool isShadowCaster = true, bool isLit = true) {

        this.textures = new Texture2D[3] { diffuse, normal, roughness };
        this.position = new Vector2(0);
        this.offset = offset;
        this.offsetToCenter = new Vector2(0);
        this.rotation = rotation;
        this.material = material;
        this.layer = layer; //layer0 = behind everything
        this.isShadowCaster = isShadowCaster;
        this.isLit = isLit;
        this.scale = scale;
        calculateOffset();
        calculateSize();
    }

    public Sprite(float rotation, Vector2 scale, Texture2D diffuse, Texture2D? normal, Texture2D? roughness, int layer = 0, int material = 0, bool isShadowCaster = true, bool isLit = true)
    {

        this.textures = new Texture2D[3] { diffuse, normal, roughness };
        this.position = new Vector2(0);
        this.offset = Vector2.Zero;
        this.offsetToCenter = new Vector2(0);
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
        //Out.PrintLn(this.InstanceID.ToString());
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        this.position.X = this.Node.GetGlobalPosition().X;
        this.position.Y = this.Node.GetGlobalPosition().Y;
        //base.OnUpdate(gameTime);
    }

    public Vector2 getPosition()
    {
        return position + offset;
    }


    internal virtual void calculateOffset()
    {
        offsetToCenter = new Vector2(textures[0].Width / 2, textures[0].Height / 2);
    }

    internal void calculateSize()
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
            spriteBatch.Draw(textures[0], new Rectangle((position+offset).ToPoint(), size), null, Color.Black, rotation, offsetToCenter, SpriteEffects.None, 0);//draw occluder mask
            return;
        }
        if (textures[textureIndex] != null)
        {
            spriteBatch.Draw(textures[textureIndex], new Rectangle((position+offset).ToPoint(), size), null, Color.White, rotation, offsetToCenter, SpriteEffects.None, 0);
        }
        else
        {
            spriteBatch.Draw(textures[0], new Rectangle((position + offset).ToPoint(), size), null, Color.Black, rotation, offsetToCenter, SpriteEffects.None, 0);//use diffuse as mask when no normal/diffuse texture provided
        }
    }

    protected override void OnDestroy()
    {
        Game.Services.GetService<IPebbleRendererService>().removeSprite(this);
    }



}


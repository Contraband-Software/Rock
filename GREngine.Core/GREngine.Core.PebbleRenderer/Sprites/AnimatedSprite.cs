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

public class AnimatedSprite : Sprite
{
    private float frameCount = 0;
    private int ssSize = 3;
    private float updateThreshhold = 20;
    private int index = 0;

    private int ssCellRes;
    public AnimatedSprite(Vector2 offset, float rotation, Vector2 scale, Texture2D diffuse, Texture2D? normal, Texture2D? roughness, int layer = 0, int material = 0, bool isShadowCaster = true, bool isLit = true) : base(rotation,scale,diffuse,normal,roughness,layer,material,isShadowCaster,isLit)
    {

        this.textures = new Texture2D[3] { diffuse, normal, roughness };
        this.position = new Vector2(0);
        this.offset = offset;
        this.offsetToCeneter = new Vector2(0);
        this.rotation = rotation;
        this.material = material;
        this.layer = layer; //layer0 = behind everything
        this.isShadowCaster = isShadowCaster;
        this.isLit = isLit;
        this.scale = scale;
        calculateOffset();
        calculateSize();

        ssCellRes = (int)MathF.Floor(textures[0].Width / ssSize);
    }

    public AnimatedSprite(float rotation, Vector2 scale, Texture2D diffuse, Texture2D? normal, Texture2D? roughness, int layer = 0, int material = 0, bool isShadowCaster = true, bool isLit = true) : base(rotation, scale, diffuse, normal, roughness, layer, material, isShadowCaster, isLit)
    {

        this.textures = new Texture2D[3] { diffuse, normal, roughness };
        this.position = new Vector2(0);
        this.offset = Vector2.Zero;
        this.offsetToCeneter = new Vector2(0);
        this.rotation = rotation;
        this.material = material;
        this.layer = layer; //layer0 = behind everything
        this.isShadowCaster = isShadowCaster;
        this.isLit = isLit;
        this.scale = scale;
        calculateOffset();
        calculateSize();

        ssCellRes = (int)MathF.Floor(textures[0].Width / ssSize);
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
    }



    //private void calculateOffset()
    //{
    //    offsetToCeneter = new Vector2(textures[0].Width / 2, textures[0].Height / 2);
    //}

    //private void calculateSize()
    //{
    //    size.X = (int)Math.Floor(textures[0].Width * scale.X);
    //    size.Y = (int)Math.Floor(textures[0].Height * scale.Y);
    //}


    //public void setScale(Vector2 scale)
    //{
    //    this.scale = scale;
    //    calculateSize();
    //}

    public override void draw(SpriteBatch spriteBatch, int textureIndex)
    {
        this.frameCount++;
        if (textureIndex == 4)
        {
            spriteBatch.Draw(textures[0], new Rectangle((position + offset).ToPoint(), size), new Rectangle((index%ssSize)*ssCellRes, (index / ssSize) * ssCellRes,ssCellRes,ssCellRes), Color.Black, rotation, offsetToCeneter, SpriteEffects.None, 0);//draw occluder mask
            return;
        }
        if (textures[textureIndex] != null)
        {
            spriteBatch.Draw(textures[textureIndex], new Rectangle((position + offset).ToPoint(), size), new Rectangle((index % ssSize) * ssCellRes, (index / ssSize) * ssCellRes, ssCellRes, ssCellRes), Color.White, rotation, offsetToCeneter, SpriteEffects.None, 0);
        }
        else
        {
            spriteBatch.Draw(textures[0], new Rectangle((position + offset).ToPoint(), size), new Rectangle((index % ssSize) * ssCellRes, (index / ssSize) * ssCellRes, ssCellRes, ssCellRes), Color.Black, rotation, offsetToCeneter, SpriteEffects.None, 0);//use diffuse as mask when no normal/diffuse texture provided
        }

        if(frameCount > updateThreshhold)
        {
            frameCount = 0;
            index++;
            if(index >= ssSize*ssSize) { //?
                index = 0;
            }
        }
    }

    internal override void calculateOffset()
    {
        offsetToCeneter = new Vector2((textures[0].Width/ssSize) / 2, (textures[0].Height / ssSize) / 2);
    }

    protected override void OnDestroy()
    {
        Game.Services.GetService<IPebbleRendererService>().removeSprite(this);
    }



}


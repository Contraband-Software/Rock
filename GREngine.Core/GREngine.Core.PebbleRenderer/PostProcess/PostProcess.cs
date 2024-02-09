#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GREngine.Core.PebbleRenderer;

public class PostProcess
{
    internal Game game;
    public Dictionary<string, float> attributes;
    public Effect? shader; //I should maybe have this non nullable


    public PostProcess(Game game, Effect? shader = null)
    {
        this.game = game;
        attributes = new Dictionary<string, float>();
        this.shader = shader;
    }

    public virtual void applyPostProcess(RenderTarget2D bufferIn, RenderTarget2D bufferOut, SpriteBatch spriteBatch)
    {
        game.GraphicsDevice.SetRenderTarget(bufferOut);

        if(this.shader == null)
        {
            spriteBatch.Begin();
        }
        else
        {
            spriteBatch.Begin(effect: shader);
            SetAllParams();
        }
 
        spriteBatch.Draw(bufferIn, new Vector2(0f), Color.White);
        spriteBatch.End();
        game.GraphicsDevice.SetRenderTarget(null);//?
    }

    public virtual void SetAllParams()
    {
        foreach (KeyValuePair<string, float> atribute in attributes)
        {
            shader.Parameters[atribute.Key]?.SetValue(atribute.Value);
        }
    }
        


}


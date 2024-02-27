#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GREngine.Core.PebbleRenderer;

public class DitherPostProcess : PostProcess
{
    Texture2D ditherPattern;
    public DitherPostProcess(Game game, Effect shader, Texture2D ditherPattern) : base(game, shader)
    {
        this.game = game;
        attributes = new Dictionary<string, float>();
        this.shader = shader;
        this.ditherPattern = ditherPattern;
    }

    public override void applyPostProcess(RenderTarget2D bufferIn, RenderTarget2D bufferOut, SpriteBatch spriteBatch)
    {
        game.GraphicsDevice.SetRenderTarget(bufferOut);

        spriteBatch.Begin(effect: shader);
        SetAllParams();

        spriteBatch.Draw(bufferIn, new Vector2(0f), Color.White);
        spriteBatch.End();
        game.GraphicsDevice.SetRenderTarget(null);//?
    }

    public override void SetAllParams()
    {
        foreach (KeyValuePair<string, float> atribute in attributes)
        {
            shader.Parameters[atribute.Key]?.SetValue(atribute.Value);
        }
        shader.Parameters["ditherSampler"]?.SetValue(ditherPattern);
    }



}


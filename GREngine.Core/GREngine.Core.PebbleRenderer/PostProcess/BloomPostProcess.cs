#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GREngine.Core.PebbleRenderer;


internal class BloomPostProcess : PostProcess // A very fast blur effect that leverages fixed function gpu texture sampling to blur with a large kernel efficeintly
                                                // input gets downsampled to size/2^passes and upsampled again. 
{
    RenderTarget2D[] BlurTargets;
    int nearestPOTx;
    int nearestPOTy;
    int width;
    int height;
    int passes;
    float finalScaleX;
    float finalScaleY;
    float scale;

    Effect isolate;
    public BloomPostProcess(Game game,Effect isolate, int width, int height, int passes, float scale) : base(game, null)
    {
        this.width = width;
        this.height = height;
        this.scale = scale;
        nearestPOTx = (int)Math.Pow(2, (int)Math.Floor(Math.Log(width, 2)));
        nearestPOTy = (int)Math.Pow(2, (int)Math.Floor(Math.Log(height, 2)));
        finalScaleX = (float)width / (float)nearestPOTx;
        finalScaleY = (float)height / (float)nearestPOTy;
        this.passes = passes;
        this.isolate = isolate;
        BlurTargets = new RenderTarget2D[passes];

        for (int i = 0; i < passes; i++)
        {
            BlurTargets[i] = new RenderTarget2D(game.GraphicsDevice, nearestPOTx / (int)Math.Pow(1 / scale, i), nearestPOTy / (int)Math.Pow(1 / scale, i));
        }

    }

    public override void applyPostProcess(RenderTarget2D bufferIn, RenderTarget2D bufferOut, SpriteBatch spriteBatch)
    {


        game.GraphicsDevice.SetRenderTarget(BlurTargets[0]);
        spriteBatch.Begin(effect: isolate);
        spriteBatch.Draw(bufferIn, new Vector2(0f), null, Color.White, 0, new Vector2(0), new Vector2(1 / finalScaleX, 1 / finalScaleY), SpriteEffects.None, 0);
        spriteBatch.End();

        for (int i = 0; i < passes - 1; i++)
        {
            game.GraphicsDevice.SetRenderTarget(BlurTargets[i + 1]);
            spriteBatch.Begin();
            spriteBatch.Draw(BlurTargets[i], new Vector2(0f), null, Color.White, 0, new Vector2(0), new Vector2(scale, scale), SpriteEffects.None, 0);
            spriteBatch.End();

        }
        for (int i = passes - 1; i > 0; i--)
        {
            game.GraphicsDevice.SetRenderTarget(BlurTargets[i - 1]);
            spriteBatch.Begin();
            spriteBatch.Draw(BlurTargets[i], new Vector2(0f), null, Color.White, 0, new Vector2(0), new Vector2(1 / scale, 1 / scale), SpriteEffects.None, 0);
            spriteBatch.End();

        }


        game.GraphicsDevice.SetRenderTarget(bufferOut);
        spriteBatch.Begin();
        spriteBatch.Draw(bufferIn, new Vector2(0f), Color.White);
        spriteBatch.End();

        spriteBatch.Begin(blendState: BlendState.Additive);
        spriteBatch.Draw(BlurTargets[0], new Vector2(0f), null, Color.White, 0, new Vector2(0), new Vector2(finalScaleX, finalScaleY), SpriteEffects.None, 0);
        spriteBatch.End();



        game.GraphicsDevice.SetRenderTarget(null);//?
    }
}


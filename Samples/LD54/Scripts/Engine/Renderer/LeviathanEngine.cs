namespace LD54.Engine.Leviathan;

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame;

interface ILeviathanEngineService
{
    public void addSprite(LeviathanSprite sprite);

    public void removeSprite(LeviathanSprite index);

    public int AddLight(Vector2 position, Vector3 color);

    public void removeLight(int id);

    public void setLightColor(int id, Color color);

    public void SetLightPosition(int id, Vector2 position);


    public Vector3 getLightColor(int id);

    public Vector2 getLightPosition(int id);

    public void updateLightPosition(int id, Vector2 offset);

    public void addPostProcess(LeviathanShader shader);

    public void removePostProcess(LeviathanShader shader);

    public void addUISprite(LeviathanUIElement uiSprite);

    public void removeUISprite(LeviathanUIElement index);

    public Vector2 getWindowSize();

    public void SetCameraPosition(Vector2 position);

    public Vector2 GetCameraPosition();

    public void bindShader(LeviathanShader shader);

    public void DebugDrawCircle(Vector2 position, float radius, Color color);

    public void DebugDrawLine(Vector2 start, Vector2 end, Color color);

    public void UnbindShaders();
}

public struct DebugCircle
{
    public DebugCircle(Vector2 center, float radius, Color color)
    {
        this.center = center;
        this.radius = radius;
        this.color = color;
    }
    public Vector2 center;
    public float radius;
    public Color color;
}

public struct DebugLine
{
    public DebugLine(Vector2 start, Vector2 end, Color color)
    {
        this.start = start;
        this.end = end;
        this.color = color;
    }
    public Vector2 start;
    public Vector2 end;
    public Color color;
}

public class LeviathanEngine : DrawableGameComponent, ILeviathanEngineService
{
    private List<DebugCircle> debugCircle = new List<DebugCircle>();
    private List<DebugLine> debugLine = new List<DebugLine>();

    private Vector2[] lightPositions = new Vector2[64];
    private Vector3[] lightColors = new Vector3[64];
    private List<LeviathanShader> shaders = new List<LeviathanShader>();
    private Queue<int> openLocations = new Queue<int>();
    private IGraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Effect lightingShader;
    private Game game;
    public Vector2 cameraPosition = new Vector2(0);
    private Texture2D blankNormal;

    RenderTarget2D colorTarget;
    RenderTarget2D normalTarget;
    RenderTarget2D shadowTarget;
    RenderTarget2D litTarget;
    RenderTarget2D unlitTarget;
    RenderTarget2D postProcessTarget;
    private bool pingpong = false;

    public List<LeviathanSprite> sprites = new List<LeviathanSprite>();
    public List<LeviathanUIElement> uiSprites = new List<LeviathanUIElement>();
    public List<LeviathanShader> postProcessShaders = new List<LeviathanShader>();

    public LeviathanEngine(Game g) : base(g)
    {
        game = g;
        graphics = g.Services.GetService<IGraphicsDeviceManager>();
        //graphics.GraphicsProfile = GraphicsProfile.HiDef;
        for (int i = 0; i < 64; i++)
        {
            openLocations.Enqueue(i);
        }

    }

    public override void Initialize()
    {
        colorTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        normalTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        shadowTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        litTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        postProcessTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);
        unlitTarget = new RenderTarget2D(game.GraphicsDevice,
            game.GraphicsDevice.PresentationParameters.BackBufferWidth,
            game.GraphicsDevice.PresentationParameters.BackBufferHeight);

        spriteBatch = new SpriteBatch(game.GraphicsDevice);
        lightingShader = game.Content.Load<Effect>("Shaders/lighting");
        blankNormal = game.Content.Load<Texture2D>("Sprites/blank");
    }
    public void UnbindShaders()
    {
        shaders.Clear();
        postProcessShaders.Clear();
    }


    public void DebugDrawCircle(Vector2 position, float radius, Color color)
    {
        debugCircle.Add(new DebugCircle(position, radius, color));
    }
    public void DebugDrawLine(Vector2 start, Vector2 end, Color color)
    {
        debugLine.Add(new DebugLine(start, end, color));
    }

    public void SetCameraPosition(Vector2 position)
    {
        this.cameraPosition = position;
    }
    public Vector2 GetCameraPosition()
    {
        return this.cameraPosition;
    }

    public Vector2 getWindowSize()
    {
        return new Vector2(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height);
    }

    public void bindShader(LeviathanShader shader)
    {
        shaders.Add(shader);
    }

    public void addPostProcess(LeviathanShader shader)
    {
        postProcessShaders.Add(shader);
    }
    public void removePostProcess(LeviathanShader shader)
    {
        postProcessShaders.Remove(shader);
    }
    public void addSprite(LeviathanSprite sprite)
    {
        sprites.Add(sprite);
    }
    public void addUISprite(LeviathanUIElement uiSprite)
    {
        uiSprites.Add(uiSprite);
    }
    public void removeSprite(LeviathanSprite sprite)
    {
        sprites.Remove(sprite);
    }
    public void removeUISprite(LeviathanUIElement uiSprite)
    {
        uiSprites.Remove(uiSprite);
    }

    public override void Draw(GameTime gameTime)
    {
        Matrix view = Matrix.Identity * Matrix.CreateTranslation(-cameraPosition.X,-cameraPosition.Y, 0);

        int width = game.GraphicsDevice.Viewport.Width;
        int height = game.GraphicsDevice.Viewport.Height;
        Matrix projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, 0, 1);
        Vector3 translation;
        view.Decompose(out _, out _, out translation);

        game.GraphicsDevice.SetRenderTarget(colorTarget);
        game.GraphicsDevice.Clear(Color.Black);
        for (int i = this.shaders.Count; i >= 0; i--)
        {
            if(i == 0)
            {
                spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, transformMatrix: view);
            }
            else
            {
                shaders[i - 1].shader.Parameters["viewProjection"]?.SetValue(view*projection);
                shaders[i - 1].shader.Parameters["time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
                shaders[i - 1].shader.Parameters["width"]?.SetValue(width);
                shaders[i - 1].shader.Parameters["height"]?.SetValue(height);
                shaders[i - 1].shader.Parameters["cpos"]?.SetValue(cameraPosition);
                shaders[i - 1].SetAllParams();
                spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, transformMatrix: view, effect: shaders[i-1].shader) ;
            }
            foreach (LeviathanSprite sprite in sprites)
            {
                if (sprite.shader == i)
                {

                    spriteBatch.Draw(sprite.color, new Rectangle(sprite.GetPositionXY().ToPoint()+ (sprite.size/2f).ToPoint(), sprite.size.ToPoint()), null, Color.White, sprite.rotation, new Vector2(sprite.color.Width / 2, sprite.color.Height / 2), SpriteEffects.None, sprite.getDepth());
                }
            }
            spriteBatch.End();
        }
        spriteBatch.Begin(transformMatrix: view);
        foreach (DebugCircle circle in debugCircle)
        {
            spriteBatch.DrawCircle(circle.center, circle.radius, 128, circle.color,4);
        }
        foreach (DebugLine line in debugLine)
        {
            spriteBatch.DrawLine(line.start, line.end, line.color,4);
        }
        spriteBatch.End();
        game.GraphicsDevice.SetRenderTarget(unlitTarget);
        game.GraphicsDevice.Clear(Color.White);
        spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, transformMatrix: view);
        foreach (LeviathanSprite sprite in sprites)
        {
            if (!sprite.isLit)
            {
                spriteBatch.Draw(sprite.color, new Rectangle(sprite.GetPositionXY().ToPoint() + (sprite.size / 2f).ToPoint(), sprite.size.ToPoint()), null, Color.Black, sprite.rotation, new Vector2(sprite.color.Width / 2, sprite.color.Height / 2), SpriteEffects.None, sprite.getDepth());
            }
            else
            {
                spriteBatch.Draw(sprite.color, new Rectangle(sprite.GetPositionXY().ToPoint() + (sprite.size / 2f).ToPoint(), sprite.size.ToPoint()), null, Color.White, sprite.rotation, new Vector2(sprite.color.Width / 2, sprite.color.Height / 2), SpriteEffects.None, sprite.getDepth());
            }
        }
        spriteBatch.End();

        game.GraphicsDevice.SetRenderTarget(normalTarget);
        game.GraphicsDevice.Clear(new Color(0.5f, 0.5f, 1f));
        spriteBatch.Begin(transformMatrix: view);

        foreach (LeviathanSprite sprite in sprites)
        {
            if (sprite.useNormal && sprite.isLit)
            {
                spriteBatch.Draw(sprite.normal, new Rectangle(sprite.GetPositionXY().ToPoint() + (sprite.size / 2f).ToPoint(), sprite.size.ToPoint()), null, Color.White, sprite.rotation, new Vector2(sprite.normal.Width / 2, sprite.normal.Height / 2), SpriteEffects.None, sprite.getDepth());
            }
            else
            {
                //spriteBatch.Draw(blankNormal, new Rectangle(sprite.GetPositionXY().ToPoint() + (sprite.size / 2f).ToPoint(), sprite.size.ToPoint()), null, Color.White, sprite.rotation, new Vector2(blankNormal.Width / 2, blankNormal.Height / 2), SpriteEffects.None, sprite.getDepth());
            }
        }

        spriteBatch.End();

        //game.GraphicsDevice.SetRenderTarget(shadowTarget);
        //game.GraphicsDevice.Clear(Color.White);
        //spriteBatch.Begin(transformMatrix: view);

        //foreach (LeviathanSprite sprite in sprites)
        //{
        //    if (sprite.isOccluder)
        //    {
        //        spriteBatch.Draw(sprite.color, new Rectangle(sprite.GetPositionXY().ToPoint() + (sprite.size / 2f).ToPoint(), sprite.size.ToPoint()), null, Color.Black, sprite.rotation, new Vector2(sprite.color.Width / 2, sprite.color.Height / 2), SpriteEffects.None, sprite.getDepth());
        //    }
        //}

        //spriteBatch.End();

        lightingShader.Parameters["translation"]?.SetValue(new Vector2(translation.X, translation.Y));
        lightingShader.Parameters["viewProjection"]?.SetValue(projection);
        lightingShader.Parameters["time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
        lightingShader.Parameters["width"]?.SetValue(width);
        lightingShader.Parameters["height"]?.SetValue(height);
        lightingShader.Parameters["lightColors"]?.SetValue(lightColors);
        lightingShader.Parameters["lightPositions"]?.SetValue(lightPositions);
        lightingShader.Parameters["normalSampler"]?.SetValue(normalTarget);
        lightingShader.Parameters["occluderSampler"]?.SetValue(shadowTarget);
        lightingShader.Parameters["unlitSampler"]?.SetValue(unlitTarget);



        game.GraphicsDevice.SetRenderTarget(litTarget);
        game.GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin(effect: lightingShader);
        spriteBatch.Draw(colorTarget, new Vector2(0), Color.White);
        spriteBatch.End();

        game.GraphicsDevice.SetRenderTarget(null);
        game.GraphicsDevice.Clear(Color.Black);

        foreach (LeviathanShader postProcess in postProcessShaders)
        {
            postProcess.shader.Parameters["viewProjection"]?.SetValue(projection);
            postProcess.shader.Parameters["time"]?.SetValue((float)gameTime.TotalGameTime.TotalSeconds);
            postProcess.shader.Parameters["width"]?.SetValue(width);
            postProcess.shader.Parameters["height"]?.SetValue(height);
            postProcess.SetAllParams();
            if (pingpong)
            {
                game.GraphicsDevice.SetRenderTarget(litTarget);
            }
            else
            {
                game.GraphicsDevice.SetRenderTarget(postProcessTarget);
            }

            game.GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(effect: postProcess.shader);
            if (pingpong)
            {
                spriteBatch.Draw(postProcessTarget, new Vector2(0), Color.White);
            }
            else
            {
                spriteBatch.Draw(litTarget, new Vector2(0), Color.White);
            }
            spriteBatch.End();
            pingpong = !pingpong;
        }
        game.GraphicsDevice.SetRenderTarget(null);
        game.GraphicsDevice.Clear(Color.Black);
        spriteBatch.Begin();
        if (pingpong)
        {
            spriteBatch.Draw(postProcessTarget, new Vector2(0), Color.White);
        }
        else
        {
            spriteBatch.Draw(litTarget, new Vector2(0), Color.White);
        }

        foreach(LeviathanUIElement uISprite in uiSprites)
        {
            if(uISprite.isEnabled)
            {
                if (uISprite.isText)
                {
                    spriteBatch.DrawString(uISprite.font, uISprite.text,uISprite.GetPositionXY(),uISprite.textColor,0,new Vector2(0),uISprite.size,SpriteEffects.None,0);
                }
                else
                {
                    spriteBatch.Draw(uISprite.color, new Rectangle(uISprite.GetPositionXY().ToPoint(), new Point((int)uISprite.size.X, (int)uISprite.size.X)), Color.White);
                    //spriteBatch.Draw(uISprite.color, new Rectangle(uISprite.GetPositionXY().ToPoint(), uISprite.size), null, Color.White, 0.05f, new Vector2(uISprite.size.X / 2, uISprite.size.Y / 2)+ uISprite.GetPositionXY(), SpriteEffects.None, 0);
                }
            }
        }

        spriteBatch.End();

        debugCircle.Clear();
        debugLine.Clear();
    }


    public int AddLight(Vector2 position, Vector3 color)
    {
        int location = openLocations.Dequeue();
        lightPositions[location] = position;
        lightColors[location] = color;
        return location;
    }
    public void removeLight(int id)
    {
        lightColors[id] = new Vector3(0);
        openLocations.Enqueue(id);
    }

    public void setLightColor(int id, Color color)
    {
        lightColors[id] = color.ToVector3();
    }
    public void SetLightPosition(int id, Vector2 position)
    {
        lightPositions[id] = position;
    }
    public Vector3 getLightColor(int id)
    {
        return lightColors[id];
    }
    public Vector2 getLightPosition(int id)
    {
        return lightPositions[id];
    }
    public void updateLightPosition(int id, Vector2 offset)
    {
        lightPositions[id] += offset;
    }
}

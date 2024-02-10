#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Formats.Asn1.AsnWriter;
using System.Net.Mime;
using MonoGame;

namespace GREngine.Core.PebbleRenderer;

public interface IPebbleRendererService
{
    public void addSprite(Sprite sprite);

    public void removeSprite(Sprite sprite);

    public void addLight(Light light);

    public void removeLight(Light light);

    public void addPostProcess(PostProcess postProcess);

    public void removePostProcess(PostProcess postProcess);

    public void setCameraPosition(Vector2 position);

    public Vector2 getCameraPosition();

    public void addMaterial(Material material);

    public void drawDebug(DebugDrawable drawable);


}

public enum DebugShape
{
    CIRCLE,
    LINE,
    RECTANGLE
}
public struct DebugDrawable
{
    public Color color;
    public Vector2 position;
    public DebugShape shape;

    public Vector2 position2;

    public DebugDrawable(Vector2 position, float radius,Color color)
    { //circle
        this.position = position;
        this.color = color;
        this.position2 = new Vector2(radius,0);
        this.shape = DebugShape.CIRCLE;
    }

    public DebugDrawable(Vector2 v1, Vector2 v2, Color color, DebugShape shape)
    {//line or rect
        this.position = v1;
        this.color = color;
        this.position2 = v2;
        this.shape = shape;
    }
}

public class PebbleRenderer : GameComponent, IPebbleRendererService
{
    const int MAX_MATERIALS = 16;//very conservative numbers
    const int MAX_lAYERS = 8;
    const int REFERENCE_WIDTH = 1920;
    const int REFERENCE_HEIGHT = 1080;

    private GraphicsDeviceManager graphics;

    private readonly int outputWidth;
    private readonly int outputHeight;
    private readonly int renderWidth;
    private readonly int renderHeight;
    private readonly float renderScale;
    private readonly float scaleFactor;
    private readonly float shadowRenderScale;

    private List<Sprite>[,] lmsTensor;

    private Dictionary<Light,Tuple<RenderTarget2D,RenderTarget2D>> lights;

    private Material[] materials;
    private int materialCount = 1; //the default matarial/shaders


    private SpriteBatch spriteBatch;

    private RenderTarget2D? output; // null by default, will get set to something for offscreen rendering

    //gbuffer
    private RenderTarget2D diffuseTarget;
    private RenderTarget2D normalTarget;
    private RenderTarget2D roughnessTarget;
    private RenderTarget2D shadowCasterTarget;

    private RenderTarget2D litTarget;

    private RenderTarget2D shadowUpscaleTarget;
    private BlurPostProcess shadowBlurer;

    private RenderTarget2D noiseTarget; 

    //renderingparams
    private Vector2 cameraPosition = new Vector2(0);
    public Color ambientLightColor;
    private bool useRaymarchedShadows = true;
    private SamplerState samplerState;
    private Texture2D nullTexture;

    //shaders
    private Shader defaultNormalShader;
    private Shader defaultDiffuseShader;
    private Shader pointLightShader;
    private Shader pointLightShaderShadowed;
    private Shader gradientNoiseShader;

    //shader params
    private double time;
    private double random;
    private Random randomGen;

    //PostProcessing
    private List<PostProcess> postProcesses;
    private RenderTarget2D postProcessTarget1;
    private RenderTarget2D postProcessTarget2;
    private bool postProcessPingPong;

    //debug
    private Queue<DebugDrawable> debugShapes;

    public PebbleRenderer(Game game, GraphicsDeviceManager graphics, int outputWidth, int outputHeight,float renderScale = 1,float shadowRenderScale = 0.2f) : base(game)
    {
        this.graphics = graphics;
        this.outputWidth = outputWidth;
        this.outputHeight = outputHeight;
        this.renderWidth = (int)MathF.Floor(outputWidth*renderScale);
        this.renderHeight = (int)MathF.Floor(outputHeight * renderScale);
        this.renderScale = renderScale;
        this.shadowRenderScale = shadowRenderScale;
        this.scaleFactor = (float)renderHeight / (float)REFERENCE_HEIGHT;
        this.ambientLightColor = new Color(0.3f, 0.3f, 0.4f);
        this.randomGen = new Random();

        this.debugShapes = new Queue<DebugDrawable>();



        lmsTensor = new List<Sprite>[MAX_lAYERS,MAX_MATERIALS]; //2d array of sprite lists, one list for each material-layer combination.
        for (int i = 0; i < MAX_lAYERS; i++)
        {
            for (int j = 0; j < MAX_MATERIALS; j++)
            {
                lmsTensor[i,j] = new List<Sprite>();
            }
        }
        materials = new Material[MAX_MATERIALS];
        lights = new Dictionary<Light, Tuple<RenderTarget2D, RenderTarget2D>>();
        postProcesses = new List<PostProcess>();


        graphics.PreferredBackBufferHeight = outputHeight;
        graphics.PreferredBackBufferWidth = outputWidth;
        graphics.ApplyChanges();//?
    }

    public override void Initialize() //overide?
    {
        samplerState = SamplerState.PointClamp;

        diffuseTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight, false, SurfaceFormat.HdrBlendable, DepthFormat.None);
        normalTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight);
        roughnessTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight);

        shadowCasterTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth*2, renderHeight*2,false,SurfaceFormat.Alpha8,DepthFormat.None); //*2?
        shadowUpscaleTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight, false, SurfaceFormat.Alpha8, DepthFormat.None);

        litTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight, false,SurfaceFormat.HdrBlendable,DepthFormat.None); 

        noiseTarget = new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight, false, SurfaceFormat.Alpha8, DepthFormat.None); 

        shadowBlurer = new BlurPostProcess(Game, renderWidth, renderHeight, 4, 0.5f);

        postProcessTarget1 = new RenderTarget2D(Game.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.HdrBlendable, DepthFormat.None);
        postProcessTarget2 = new RenderTarget2D(Game.GraphicsDevice, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight, false, SurfaceFormat.HdrBlendable, DepthFormat.None);

        nullTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
        nullTexture.SetData(new Color[] { Color.White });

        spriteBatch = new SpriteBatch(Game.GraphicsDevice);
    }

    public void LoadShaders()
    {
        defaultNormalShader = new Shader(Game.Content.Load<Effect>("Graphics/defaultNormalShader")); // make sure this loads  before anything ++ add error throw
        defaultDiffuseShader = new Shader(Game.Content.Load<Effect>("Graphics/defaultDiffuseShader"));
        pointLightShader = new Shader(Game.Content.Load<Effect>("Graphics/pointLight"));
        pointLightShaderShadowed = new Shader(Game.Content.Load<Effect>("Graphics/pointLightShaderShadowCasting")); //maybe some of these can just be effects not shaders.
        gradientNoiseShader = new Shader(Game.Content.Load<Effect>("Graphics/gradientNoiseShader"));

        materials[0] = new Material(defaultDiffuseShader, defaultNormalShader, null); //default mat
    }

    public void drawDebug(DebugDrawable drawable)
    {
        debugShapes.Enqueue(drawable);
    }
    public void setCameraPosition(Vector2 cameraPostion)
    {
        this.cameraPosition = cameraPostion;
    }
    public Vector2 getCameraPosition()
    {
        return cameraPosition;
    }

    public void addSprite(Sprite sprite)
    {
        if (sprite.material >= materialCount)
        {
            throw new ArgumentException("shader uses material Index: " + sprite.material+" but this shader is not registered.");
        }
        lmsTensor[sprite.layer, sprite.material].Add(sprite);

    }

    public void removeSprite(Sprite sprite)
    {
        lmsTensor[sprite.layer, sprite.material].Remove(sprite);
    }

    public void addLight(Light light)
    {
        if(light.isShadowCasting)
        {
            lights.Add(light, new Tuple<RenderTarget2D,RenderTarget2D>(
                new RenderTarget2D(Game.GraphicsDevice, (int)MathF.Floor(renderWidth * shadowRenderScale), (int)MathF.Floor(renderHeight * shadowRenderScale), false, SurfaceFormat.Alpha8, DepthFormat.None),
                new RenderTarget2D(Game.GraphicsDevice, renderWidth, renderHeight, false, SurfaceFormat.Alpha8, DepthFormat.None)));
        }
        else
        {
            RenderTarget2D whiteTarget = new RenderTarget2D(Game.GraphicsDevice, 1, 1, false, SurfaceFormat.Alpha8, DepthFormat.None);
            Game.GraphicsDevice.SetRenderTarget(whiteTarget);
            Game.GraphicsDevice.Clear(Color.White);
            Game.GraphicsDevice.SetRenderTarget(null);
            lights.Add(light, new Tuple<RenderTarget2D,RenderTarget2D>(whiteTarget, whiteTarget));
        }
    }

    public void removeLight(Light light)
    {
        lights.Remove(light);
    }

    public void addPostProcess(PostProcess postProcess)
    {
        postProcesses.Add(postProcess);
    }

    public void removePostProcess(PostProcess postProcess)
    {
        postProcesses.Remove(postProcess);
    }

    public void addMaterial(Material material)
    {
        if (materialCount >= MAX_MATERIALS)
        {
            throw new ArgumentException("Maximum number of materials reached, increase MAX_MATERIALS to add more");
        }
        if (material.shaders[1] == null)// I will need this hack for roughness too ?
        {
            material.shaders[1] = defaultNormalShader;
        }
        if (material.shaders[0] == null)
        {
            material.shaders[0] = defaultDiffuseShader;
        }
        materials[materialCount] = material;
        materialCount++;
    }


    public void Draw(GameTime time) {
        this.time = time.TotalGameTime.TotalSeconds;
        this.random = randomGen.NextDouble();
        RenderNoise();

        Matrix view = Matrix.CreateTranslation(0f - cameraPosition.X, 0f - cameraPosition.Y, 0f) * Matrix.CreateScale(scaleFactor);

        renderToTarget(normalTarget, view, new Color(0.5f, 0.5f, 1f), 1);

        renderToTarget(roughnessTarget, view, Color.Black, 2);



        //maybe make drawing occlusion map conditional on having shadow casting lights?
        renderShadowCasters(shadowCasterTarget, view * Matrix.CreateScale(shadowRenderScale) * Matrix.CreateTranslation(new Vector3(renderWidth / 2, renderHeight / 2,0)));



        //shadows
        pointLightShaderShadowed.shader.Parameters["translation"]?.SetValue(-cameraPosition * scaleFactor * shadowRenderScale); //shadowrenderscale?
        setEngineShaderParams(pointLightShaderShadowed.shader);



        foreach (KeyValuePair<Light, Tuple<RenderTarget2D, RenderTarget2D>> light in lights)
        {
            if (light.Key.isShadowCasting)
            {
                Game.GraphicsDevice.SetRenderTarget(light.Value.Item1);
                Game.GraphicsDevice.Clear(Color.White);
                spriteBatch.Begin(effect: pointLightShaderShadowed.shader);
                pointLightShaderShadowed.shader.Parameters["lightPosition"]?.SetValue(light.Key.position * scaleFactor * shadowRenderScale);
                pointLightShaderShadowed.shader.Parameters["lightDirection"]?.SetValue(light.Key.getLightDir());
                spriteBatch.Draw(shadowCasterTarget, new Vector2(0), Color.White);
                spriteBatch.End();

                Game.GraphicsDevice.SetRenderTarget(shadowUpscaleTarget);
                spriteBatch.Begin();
                spriteBatch.Draw(light.Value.Item1, new Vector2(0f), null, Color.White, 0, new Vector2(0), new Vector2(1 / shadowRenderScale, 1 / shadowRenderScale), SpriteEffects.None, 0);
                spriteBatch.End();


                shadowBlurer.applyPostProcess(shadowUpscaleTarget, light.Value.Item2, spriteBatch);
            }
        }



        // lights
        pointLightShader.shader.Parameters["translation"]?.SetValue(-cameraPosition * scaleFactor);
        pointLightShader.shader.Parameters["roughnessSampler"]?.SetValue(roughnessTarget);
        setEngineShaderParams(pointLightShader.shader);
        Game.GraphicsDevice.SetRenderTarget(litTarget);

        foreach (KeyValuePair<Light,Tuple<RenderTarget2D, RenderTarget2D>> light in lights)
        {
            //the aproach I have in mind might be too cpu/ draw call intesive, so I will try raymarching first, but maybe come back to the 1d occlusion map approach with some optimisation later.
            spriteBatch.Begin(effect: pointLightShader.shader, blendState: BlendState.Additive);
            pointLightShader.shader.Parameters["shadowMapSampler"]?.SetValue(light.Value.Item2);
            pointLightShader.shader.Parameters["lightColor"]?.SetValue(light.Key.color * MathF.Sqrt(scaleFactor) * scaleFactor);//this is slightly wrong, figure out later
            pointLightShader.shader.Parameters["lightPosition"]?.SetValue(light.Key.position * scaleFactor);
            pointLightShader.shader.Parameters["lightDirection"]?.SetValue(light.Key.getLightDir());

            spriteBatch.Draw(normalTarget, new Vector2(0f), Color.White);
            spriteBatch.End();
        }
        Game.GraphicsDevice.SetRenderTarget(null);

        renderOutput(diffuseTarget, view);

        //HERE
        Game.GraphicsDevice.SetRenderTarget(postProcessTarget1); //I can maybe optimise this to overwrite diffuse buffer or lighting buffers, depends if I still want them
        spriteBatch.Begin(transformMatrix: Matrix.CreateScale(1 / renderScale),samplerState: samplerState); // maybe dont create the matrix every frame
        spriteBatch.Draw(diffuseTarget, new Vector2(0f), Color.White);
        spriteBatch.End();
        Game.GraphicsDevice.SetRenderTarget(null);
        postProcessPingPong = true;

        foreach (PostProcess postProcess in postProcesses){
            if(postProcess.shader != null)
            {
                setEngineShaderParams(postProcess.shader);
            }
            if (postProcessPingPong)
            {
                postProcess.applyPostProcess(postProcessTarget1, postProcessTarget2, spriteBatch);
            }
            else
            {
                postProcess.applyPostProcess(postProcessTarget2, postProcessTarget1, spriteBatch);
            }
            postProcessPingPong = !postProcessPingPong;
        }

        spriteBatch.Begin();
        if (postProcessPingPong)
        {
            spriteBatch.Draw(postProcessTarget1, new Vector2(0f), Color.White);
        }
        else
        {
            spriteBatch.Draw(postProcessTarget2, new Vector2(0f), Color.White);
        }
        spriteBatch.End();

        renderDebugShapes(null, view);
    }

    private void setEngineShaderParams(Effect effect)
    {
        effect.Parameters["time"]?.SetValue((float)time);
        effect.Parameters["random"]?.SetValue((float)random);
        effect.Parameters["width"]?.SetValue(renderWidth);// are these needed? should be float?
        effect.Parameters["height"]?.SetValue(renderHeight);
        effect.Parameters["noiseMap"]?.SetValue(noiseTarget);
    }

    private void renderOutput(RenderTarget2D target, Matrix viewProjection){ //refactor this maybe, having a function for rendering to target isnt as nice as I hoped.
        Game.GraphicsDevice.SetRenderTarget(target);
        Game.GraphicsDevice.Clear(Color.Black);

        for (int i = 0; i < MAX_lAYERS; i++)
        {
            for (int j = 0; j < materialCount; j++)
            {
                materials[j].shaders[0].shader.Parameters["lightMap"]?.SetValue(litTarget);
                materials[j].shaders[0].shader.Parameters["ambientColor"]?.SetValue(ambientLightColor.ToVector3());
                setEngineShaderParams(materials[j].shaders[0].shader);

                if(lmsTensor[i, j].Count == 0)
                {
                    continue;
                }
                spriteBatch.Begin(transformMatrix: viewProjection, effect: materials[j].shaders[0].shader);

                foreach (Sprite sprite in lmsTensor[i, j])
                {
                    sprite.draw(spriteBatch, 0);
                }
                spriteBatch.End();
            }
        }
        Game.GraphicsDevice.SetRenderTarget(null);// is this neccasary?
    }

    private void renderToTarget(RenderTarget2D target, Matrix viewProjection,Color clearColor, int shaderIndex)
    {
        Game.GraphicsDevice.SetRenderTarget(target);
        Game.GraphicsDevice.Clear(clearColor);

        for (int i = 0; i < MAX_lAYERS; i++)
        {
            for (int j = 0; j < materialCount; j++)
            {
                if (lmsTensor[i, j].Count == 0)
                {
                    continue;
                }
                if (materials[j].shaders[shaderIndex] == null)
                {
                    spriteBatch.Begin(transformMatrix: viewProjection);
                }
                else
                {
                    setEngineShaderParams(materials[j].shaders[shaderIndex].shader);// do I want to do this automatically, or have to add to param list for each material?
                    spriteBatch.Begin(transformMatrix: viewProjection, effect: materials[j].shaders[shaderIndex].shader);
                }

                foreach (Sprite sprite in lmsTensor[i, j])
                {
                    if (shaderIndex == 0 || sprite.isLit)//optimisation, unlit sprites dont need to draw to normal or roughness buffers
                    {
                        sprite.draw(spriteBatch, shaderIndex);
                    }
                }
                spriteBatch.End();
            }
        }
        Game.GraphicsDevice.SetRenderTarget(null);// is this neccasary?
    }

    private void RenderNoise()
    {
        Game.GraphicsDevice.SetRenderTarget(noiseTarget);
        setEngineShaderParams(gradientNoiseShader.shader);
        spriteBatch.Begin(effect: gradientNoiseShader.shader);
        spriteBatch.Draw(nullTexture, new Vector2(0),null, Color.White,0,Vector2.Zero,new Vector2(renderWidth,renderHeight),SpriteEffects.None,0);
        spriteBatch.End();
        Game.GraphicsDevice.SetRenderTarget(null);//? needed?
    }

    private void renderShadowCasters(RenderTarget2D target, Matrix viewProjection) {
        Game.GraphicsDevice.SetRenderTarget(target);
        Game.GraphicsDevice.Clear(Color.White);

        for (int i = 0; i < MAX_lAYERS; i++)
        {
            for (int j = 0; j < materialCount; j++)
            {
                if (lmsTensor[i, j].Count == 0)
                {
                    continue;
                }
                if (materials[j].shaders[0] == null)
                {
                    spriteBatch.Begin(transformMatrix: viewProjection);
                }
                else
                {
                    materials[j].shaders[0].shader.Parameters["time"]?.SetValue((float)time);// do I want to do this automatically, or have to add to param list for each material?
                    spriteBatch.Begin(transformMatrix: viewProjection, effect: materials[j].shaders[0].shader);
                }

                foreach (Sprite sprite in lmsTensor[i, j])
                {
                    if (sprite.isShadowCaster)
                    {
                        sprite.draw(spriteBatch, 4);
                    }
                }
                spriteBatch.End();
            }
        }
        Game.GraphicsDevice.SetRenderTarget(null);// is this neccasary?
    }

    private void renderDebugShapes(RenderTarget2D target, Matrix viewProjection)
    {
        Game.GraphicsDevice.SetRenderTarget(target);
        spriteBatch.Begin(transformMatrix: viewProjection);
        while(debugShapes.Count > 0)
        {
            DebugDrawable drawable = debugShapes.Dequeue();

            switch (drawable.shape)
            {
                case DebugShape.LINE:
                    spriteBatch.DrawLine(drawable.position,drawable.position2, drawable.color,2);
                    break;

                case DebugShape.RECTANGLE:
                    spriteBatch.DrawRectangle(new Rectangle((int)drawable.position.X, (int)drawable.position.Y, (int)(drawable.position2.X - drawable.position.X), (int)(drawable.position2.Y - drawable.position.Y)),drawable.color,2 );
                    break;
                case DebugShape.CIRCLE:
                    spriteBatch.DrawCircle(drawable.position,drawable.position2.X,32,drawable.color,2);
                    break;
            }
        }
        spriteBatch.End();
    }


}



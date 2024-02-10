#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

float4x4 viewProjection;
float time;


sampler2D screenSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float2 curve(float2 uv)
{
    uv = (uv - 0.5) * 2.0;
    uv *= 1.1;
    uv.x *= 1.0 + pow((abs(uv.y) / 5.0), 2.0);
    uv.y *= 1.0 + pow((abs(uv.x) / 4.0), 2.0);
    uv = (uv / 2.0) + 0.5;
    uv = uv * 0.92 + 0.04;
    return uv;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    //Curve
    float2 uv = input.TextureCoordinates;
    uv = curve(uv);
    
    float3 col;

    // Chromatic
    col.r = tex2D(screenSampler, float2(uv.x + 0.003, uv.y)).x;
    col.g = tex2D(screenSampler, float2(uv.x + 0.000, uv.y)).y;
    col.b = tex2D(screenSampler, float2(uv.x - 0.003, uv.y)).z;

    col *= step(0.0, uv.x) * step(0.0, uv.y);
    col *= 1.0 - step(1.0, uv.x) * 1.0 - step(1.0, uv.y);

    col *= 0.5 + 0.5 * 16.0 * uv.x * uv.y * (1.0 - uv.x) * (1.0 - uv.y);
    col *= float3(0.95, 1.05, 0.95);

    col *= 0.9 + 0.1 * sin(10.0 * time + uv.y * 700.0);

    col *= 0.99 + 0.01 * sin(110.0 * time);

    return float4(col, 1.0);

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
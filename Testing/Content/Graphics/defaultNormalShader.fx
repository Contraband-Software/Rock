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


sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
    float Depth : DEPTH0; //?
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 sample = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
    if (input.Color.r == 0 && input.Color.g == 0 && input.Color.b == 0 && sample.a != 0)
    {
        sample = float4(0.5, 0.5, 1, 1);
    }
    return sample;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
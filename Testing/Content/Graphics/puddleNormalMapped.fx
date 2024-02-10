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
    float2 uv = input.TextureCoordinates;
    float4 sample = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
    if ((uv.x > 0.75 && uv.y > 0.75) || sample.a == 0 )
    {
        return float4(0, 0, 0, 0);
    }

    float spriteSheetIndex = float2(0, 0);
    
    int index = floor(time*2) % 4;

    if (index == 1)
    {
        spriteSheetIndex = float2(0.125, 0);
    }
    if (index == 2)
    {
        spriteSheetIndex = float2(0, 0.125);
    }
    if (index == 3)
    {
        spriteSheetIndex = float2(0.125, 0.125);
    }

    if (dot(sample.rgb, float3(0.5, 0.5, 1)) < 1.1)
    {
        float2 puddleNormalUV = float2(0.75, 0.75) + 0.14 * (uv * 4) % 0.12 + spriteSheetIndex;
        float4 puddleNormal = tex2D(SpriteTextureSampler, puddleNormalUV);
        return float4(puddleNormal.rgb, sample.a);
    }

    
    //return float4(sample.rgb*puddleNormal.rgb,sample.a);
    return sample;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

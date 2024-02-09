#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


float4x4 viewProjection;
float time;
//float width;
//float height;
//float strength;
sampler colorSampler : register(s0);

struct VertexInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput
{
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};



PixelInput SpriteVertexShader(VertexInput v)
{
    PixelInput output;

    output.Position = mul(v.Position, viewProjection);
    output.Color = v.Color;
    output.TexCoord = v.TexCoord;
    
    return output;
}

float4 SpritePixelShader(PixelInput p) : SV_TARGET
{
    float4 col = float4(0, 0, 0, 1);
    
    float2 uv = p.TexCoord.xy + float2(sin(time), sin(time)) * 0.05;
    
    col = float4(tex2D(colorSampler, uv).rgb, 1.0);
    return col;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_SHADERMODELSpriteVertexShader();
        PixelShader = compile PS_SHADERMODELSpritePixelShader();
    }
}

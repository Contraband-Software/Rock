#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


float4x4 viewProjection;
float time;
float width;
float height;
float strength;
float brightnessThreshold;
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
    float oneOverWidth = (1 / width)*2;
    float4 col = tex2D(colorSampler, p.TexCoord.xy);

    float brightness = 0;
    float3 avarageColor = float3(0, 0, 0);
    
    for (int i = -4; i < 5; i++)
    {
        for (int j = -3; j < 4; j++)
        {
            float3 sample = tex2D(colorSampler, p.TexCoord.xy + float2(i * oneOverWidth, j * oneOverWidth)).rgb;
            avarageColor += sample;
            brightness = brightness +sample.r + sample.g + sample.b;
        }
    }    
    if (brightness > brightnessThreshold)
    {
        return col + float4(avarageColor * strength / 64, 0) * length(avarageColor);
    }
    return col;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}

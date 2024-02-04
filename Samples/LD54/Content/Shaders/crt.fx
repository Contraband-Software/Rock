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

float hash(float p)
{
    p = frac(p * .1031);
    p *= p + 33.33;
    p *= p + p;
    return frac(p);
}
float hash12(float2 p)
{
    float3 p3 = frac(float3(p.xyx) * .1031);
    p3 += dot(p3, p3.yzx + 33.33);
    return frac((p3.x + p3.y) * p3.z);
}


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

    float warp = 0.5;
    float scan = 0.75;
    float2 uv = p.TexCoord.xy;
    float2 dc = abs(0.5 - p.TexCoord.xy);
    float dist = 1.075- length(float2(dc.x * 0.15, dc.y * 0.2));
    dist = min(pow(dist, 60), 1);
    dc *= dc;

    if (hash(floor(uv.y * 30)/30 + time) > 0.96)
    {
        uv = float2(uv.x + hash(time + uv.y) * 0.003, uv.y);
    }
    else
    {
        uv = float2(uv.x + hash(time + uv.y) * 0.0008, uv.y);
    }
    
    uv.x -= 0.5;
    uv.x *= 1.0 + (dc.y * (0.3 * warp));
    uv.x += 0.5;
    uv.y -= 0.5;
    uv.y *= 1.0 + (dc.x * (0.4 * warp));
    uv.y += 0.5;

    
        if (uv.y > 1.0 || uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0)
    {
        col = float4(0.0, 0.0, 0.0, 1.0);
    }
    else
    {
        float apply = abs(sin(col.y) * 0.5 * scan);
        col = float4(tex2D(colorSampler, uv).rgb * (clamp(sin(uv.y * 1000) / 2 + 1, 0.4, 1)) * dist, 1.0);
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

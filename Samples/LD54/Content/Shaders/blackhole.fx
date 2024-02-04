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

float lengthSquared(float2 l)
{
    return pow(l.x - 0.5, 2) + pow(l.y - 0.5, 2);

}

float4 SpritePixelShader(PixelInput p) : SV_TARGET
{
    float4 col = float4(0, 0, 0, 0);
    float lengthSq = lengthSquared(p.TexCoord.xy);

    if (lengthSq < 0.001 + tex2D(colorSampler, p.TexCoord.xy * 0.4 + float2((time * 0.2) % 0.25, (time * 0.2) % 0.25)).r * 0.002)
    {
        col = float4(0, 0, 0, 1);

    }
    else
    {
        float2 centerToPixel = normalize(p.TexCoord.xy - float2(0.5, 0.5));
        float cosine = acos(dot(centerToPixel, float2(cos(time*2), sin(time*2)))) * 0.003;

        if (lengthSq < pow(tex2D(colorSampler, p.TexCoord.xy * 0.1 + float2((time * 0.01) % 0.25, (time * 0.01) % 0.25)).r, 2) * 0.2 + tex2D(colorSampler, float2(lengthSq + (time * 0.01) % 1, cosine + (time * 0.01) % 0.5)).r * 0.06)
        {
            float3 fac = float3(1 / lengthSq, 1 / lengthSq, 1 / lengthSq) * 0.002 * float3(1, 0.3, 0.8) * (dot(float3(centerToPixel, 10), tex2D(colorSampler, p.TexCoord.xy + float2(cosine * 5 + sin(p.TexCoord.x) * 0.1, cosine * 5 + sin(p.TexCoord.y) * 0.1) * 2).xyz) / 2 + 1);
            float f = tex2D(colorSampler, float2(lengthSq + (time * 0.01) % 1, cosine + (time * 0.01) % 0.5)).r * tex2D(colorSampler, (p.TexCoord.xy*0.5 + float2((time * 0.02), (time * 0.02)))%1).r;
            float g = pow(tex2D(colorSampler, float2(lengthSq + (time * 0.01) % 1, cosine + (time * 0.01) % 0.5)).r * tex2D(colorSampler, (p.TexCoord.xy * 0.5 + float2((time * 0.02), (time * 0.02))) % 1).r*4, 4) * 0.2;

            float alpha = 0;
            if (f < 0.28 || g>0.5)
            {
                alpha = 1;
            }

           
            if (g > 0.5)
            {
                fac = fac *10;
            }

            col = float4(fac.r, fac.g, fac.b, alpha);
        }
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

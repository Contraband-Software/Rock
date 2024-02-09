#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

int width;
int height;
float time;
float3 ambientColor;


sampler diffuseSampler : register(s0);
sampler lightMap : register(s1);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
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


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float val = floor(hash(input.Position.y +time ) * 10) / 10;
    float2 ligtmapUV = float2(input.Position.x / width, input.Position.y / height) + float2(val, val) * 0.0;
    float3 lightColor = tex2D(lightMap, ligtmapUV).rgb + ambientColor; //baked in ambient todo make it a parameter
	
    float2 uv = input.TextureCoordinates;
    float4 sample = tex2D(diffuseSampler, uv  + float2(sin(time + uv.x * 10), sin(time + uv.y * 10)) * 0.05);
    return float4(sample.rgb * lightColor, sample.a) * input.Color; //a is alpha right?	
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};
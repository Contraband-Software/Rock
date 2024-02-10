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
sampler noiseMap : register(s2);

float rand2(float2 n)
{
    return frac(sin(dot(n, float2(12.9898, 4.1414))) * 43758.5453);
}


float gnoise(float2 n)
{
    const float2 d = float2(0.0, 1.0);
    float2 b = floor(n),
        f = smoothstep(d.xx, d.yy, frac(n));

    //float2 f = frac(n);
	//f = f*f*(3.0-2.0*f);

    float x = lerp(rand2(b), rand2(b + d.yx), f.x),
          y = lerp(rand2(b + d.xy), rand2(b + d.yy), f.x);

    return lerp(x, y, f.y);
}


struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float scaledTime = time * 0.01;
    float scale = 5;
    float waveSpeed = 250;
    
    float2 ligtmapUV = float2(input.Position.x / width, input.Position.y / height);
    float3 lightColor = tex2D(lightMap, ligtmapUV).rgb + ambientColor * 8; //baked in ambient todo make it a parameter
	
    float2 uv = input.TextureCoordinates * scale;

    float hlUVNoise = tex2D(noiseMap, uv * scale / 8).r;
    float4 sample = tex2D(diffuseSampler, uv + hlUVNoise * 0.1);

    float4 noiseSample = tex2D(noiseMap, uv);

    
    
    float waveRippleMask = sin(uv.y * scale * 4 + scaledTime * waveSpeed) + tex2D(noiseMap, uv * scale * float2(0.02, 0.02) + scaledTime).r;
    float ripple = tex2D(noiseMap, uv * scale * float2(0.12, 0.02) + scaledTime).r * waveRippleMask;
    
    float waveNoise = min(gnoise((uv + float2(0, scaledTime)) * float2(10, 100) + ripple + hlUVNoise) + 0.2, 1);
    float foam = step(0.8, waveNoise + waveRippleMask * 0.12);

    
    float3 waveRoughnes = min(float3(1 - min(foam, 1), 1 - min(foam, 1), 1 - min(foam, 1)) +0.6, float3(1, 1, 1));
    
    return float4(waveRoughnes, sample.a); //a is alpha right?
    //return float4(hlUVNoise, hlUVNoise, hlUVNoise, 1);

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

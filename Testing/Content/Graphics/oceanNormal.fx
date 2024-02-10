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


sampler SpriteTextureSampler : register(s0);

sampler noiseMap : register(s1);


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

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


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float scaledTime = time * 0.01;
    float scale = 8;
    float waveSpeed = 250;
    float waveAngle = 0.5;
    float waveSize = 1.4;
    float2 uv = input.TextureCoordinates * scale;

    //float hlUVNoise = tex2D(noiseMap, uv *scale/8).r;
    //float4 sample = tex2D(diffuseSampler, uv + hlUVNoise*0.1);

    //float noiseSample = tex2D(noiseMap, uv / 2).r; // maybe use this for better perf

    //float noiseSample2 = tex2D(noiseMap, uv * 0.1 + float2(sin(uv.x * 0.1), 0.5 * cos(uv.x * 0.1))).r;
    float noiseSample = gnoise(uv * scale);

    float noiseSample2 = gnoise(uv * 0.8 + float2(sin(uv.x * 0.1), 0.5 * cos(uv.x * 0.1)));
    
    noiseSample2 = pow(noiseSample2 + 0.4, 0.5);

    float noiseSample3 = tex2D(noiseMap, uv / 16 + float2(sin(uv.x / 16), 0.5 * cos(uv.x / 16))).r;
    //noiseSample3 = pow(noiseSample2 + 0.4, 0.5);
    noiseSample3 = max(pow(noiseSample3, 0.25) - 0.1, 0);

    
    
    //float waveRippleMask = sin(uv.y * scale * 4 + scaledTime * waveSpeed) + tex2D(noiseMap, uv * scale * float2(0.02, 0.02) + scaledTime).r;
    //float ripple = tex2D(noiseMap, uv * scale * float2(0.12,0.02) + scaledTime).r * waveRippleMask;
    
    //float waveNoise = gnoise((uv + float2(0, scaledTime)) * float2(10, 100) + ripple + hlUVNoise);
    //float foam = step(0.8, waveNoise + waveRippleMask*0.12);

    
    //float3 waveColor = float3(foam, foam, foam) + float3(0.2, 0.4, 0.9) * (floor(pow(waveNoise, 0.2) * 5) / 8 + 0.4);

    float2 waveMaskUV = uv + float2(scaledTime, scaledTime) + noiseSample * 0.05 * waveSize + noiseSample2 * 0.5 * waveSize + noiseSample3 * 0.5 * waveSize; //+ float2(sin(uv.x +scaledTime*10), cos(uv.y+scaledTime*10)); //+ 0.1*float2(sin(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)),cos(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)));
    
    float waveMask = 1 - ((waveMaskUV.x * sin(waveAngle) + waveMaskUV.y * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;

    float waveMaskoffset1 = 1 - (((waveMaskUV.x + 0.02 / scale) * sin(waveAngle) + (waveMaskUV.y + 0.02 / scale) * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;
    float waveMaskoffset2 = 1 - (((waveMaskUV.x + 0.04 / scale) * sin(waveAngle) + (waveMaskUV.y + 0.04 / scale) * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;


    waveMask = pow(waveMask, 2);

    
    waveMask += pow(waveMaskoffset1, 2);
    waveMask += pow(waveMaskoffset2, 2);

    waveMask *= pow(noiseSample, 2);

    //waveMask *= 2;
    
    waveMask = min(waveMask, 1);

    waveMask = floor(waveMask * 16) / 16;

    float3 waveColor = waveMask + pow(waveMask + 0.3, 0.5) * float3(0.047, 0.086, 0.2) * 2;

    //waveMask = smoothstep(0.4,1,waveMask);

    
    float4 sample = tex2D(SpriteTextureSampler, (waveMaskUV + time * 0.01) % 1);
    return float4(sample.rgb * (waveMask + 0.2) + float3(0.5, 0.5, 1) * (1 - waveMask), 1);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

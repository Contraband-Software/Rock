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

float2 hash(float2 p)
{
    //p = mod(p, 4.0); // tile
    p = float2(dot(p, float2(127.1, 311.7)),
             dot(p, float2(269.5, 183.3)));
    return frac(sin(p) * 18.5453);
}

// return distance, and cell id
float voronoi(float2 x)
{
    float2 n = floor(x);
    float2 f = frac(x);

    float3 m = float3(8,8,8);
    for (int j = -1; j <= 1; j++)
        for (int i = -1; i <= 1; i++)
        {
            float2 g = float2(float(i), float(j));
            float2 o = hash(n + g);
      //vec2  r = g - f + o;
            float2 r = g - f + (0.5 + 0.5 * sin(time + 6.2831 * o));
            float d = dot(r, r);
            if (d < m.x)
                m = float3(d, o);
        }

    return sqrt(m.x);
}


struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float scaledTime = time *0.1;
    float scale = 4;
    float waveSpeed = 250;
    float waveAngle = 0.5;
    
    float2 ligtmapUV = float2(input.Position.x / width, input.Position.y / height);
    float3 lightColor = tex2D(lightMap, ligtmapUV).rgb + ambientColor; //baked in ambient todo make it a parameter
	
    float2 uv = input.TextureCoordinates * scale;

    //float hlUVNoise = tex2D(noiseMap, uv *scale/8).r;
    //float4 sample = tex2D(diffuseSampler, uv + hlUVNoise*0.1);

    float noiseSample = tex2D(noiseMap, uv/2).r;

    float noiseSample2 = tex2D(noiseMap, uv*0.1 + float2(sin(uv.x*0.1), 0.5*cos(uv.x*0.1))).r;
    noiseSample2 = pow(noiseSample2 + 0.4, 0.5);

    float noiseSample3 = tex2D(noiseMap, uv/16 + float2(sin(uv.x/16), 0.5 * cos(uv.x/16))).r;
    //noiseSample3 = pow(noiseSample2 + 0.4, 0.5);
    noiseSample3 = max(pow(noiseSample3, 0.25) - 0.1, 0);

    
    
    //float waveRippleMask = sin(uv.y * scale * 4 + scaledTime * waveSpeed) + tex2D(noiseMap, uv * scale * float2(0.02, 0.02) + scaledTime).r;
    //float ripple = tex2D(noiseMap, uv * scale * float2(0.12,0.02) + scaledTime).r * waveRippleMask;
    
    //float waveNoise = gnoise((uv + float2(0, scaledTime)) * float2(10, 100) + ripple + hlUVNoise);
    //float foam = step(0.8, waveNoise + waveRippleMask*0.12);

    
    //float3 waveColor = float3(foam, foam, foam) + float3(0.2, 0.4, 0.9) * (floor(pow(waveNoise, 0.2) * 5) / 8 + 0.4);

    float2 waveMaskUV = uv + float2(scaledTime, scaledTime) + noiseSample * 0.05 + noiseSample2*0.5 + noiseSample3*0.5; //+ float2(sin(uv.x +scaledTime*10), cos(uv.y+scaledTime*10)); //+ 0.1*float2(sin(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)),cos(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)));
    
    float waveMask = 1 - ((waveMaskUV.x * sin(waveAngle) + waveMaskUV.y * cos(waveAngle)) % (1 / scale)) * scale;

    float waveMaskoffset1 = 1 - (((waveMaskUV.x + 0.01) * sin(waveAngle) + (waveMaskUV.y + 0.01) * cos(waveAngle)) % (1 / scale)) * scale;
    float waveMaskoffset2 = 1 - (((waveMaskUV.x + 0.02) * sin(waveAngle) + (waveMaskUV.y + 0.02) * cos(waveAngle)) % (1 / scale)) * scale;

    waveMask = pow(waveMask, 2);
    float vorn = voronoi(uv*scale);

    waveMask *= vorn;

    waveMask += pow(waveMaskoffset1, 4) * 0.5;
    waveMask += pow(waveMaskoffset2, 4) * 0.5;

    //waveMask *= 2;
    
    waveMask = min(waveMask, 1);
    waveMask = smoothstep(0.3, 1,waveMask);
    
    
    return float4(waveMask, waveMask, waveMask, 1) * input.Color; //a is alpha right?
    //return float4(hlUVNoise, hlUVNoise, hlUVNoise, 1);

}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

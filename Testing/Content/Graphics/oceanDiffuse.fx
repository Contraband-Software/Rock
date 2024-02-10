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

float3 scatter(float2 uv)
{
    float3 accumulatedLight = float3(0,0,0);
    for (int i = -2; i < 2; i++)
    {
        for (int j = -2; j < 2; j++)
        {
            accumulatedLight += tex2D(lightMap, (uv + float2(i, j) * 0.02)%1).rgb;

        }
    }
    return accumulatedLight / 16;
}


struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float scaledTime = time *0.01;
    float scale = 8;
    float waveSpeed = 250;
    float waveAngle = 0.5;
    float waveSize = 1.4;
    
    float2 lightmapUV = float2(input.Position.x / width, input.Position.y / height);
    //float3 lightColor = tex2D(lightMap, lightmapUV).rgb + ambientColor; //baked in ambient todo make it a parameter
    float3 lightColor = tex2D(lightMap, lightmapUV).rgb*0.1 + ambientColor +scatter(lightmapUV)*0.2;
    float2 uv = input.TextureCoordinates * scale;

    //float hlUVNoise = tex2D(noiseMap, uv *scale/8).r;
    //float4 sample = tex2D(diffuseSampler, uv + hlUVNoise*0.1);

    float noiseSample = gnoise(uv * scale);

    float noiseSample2 = gnoise(uv * 0.8 + float2(sin(uv.x * 0.1), 0.5 * cos(uv.x * 0.1)));
    noiseSample2 = pow(noiseSample2 + 0.4, 0.5);

    float noiseSample3 = tex2D(noiseMap, uv/16 + float2(sin(uv.x/16), 0.5 * cos(uv.x/16))).r;
    //noiseSample3 = pow(noiseSample2 + 0.4, 0.5);
    noiseSample3 = max(pow(noiseSample3, 0.25) - 0.1, 0);

   
    float2 waveMaskUV = uv + float2(scaledTime, scaledTime) + noiseSample * 0.05 * waveSize + noiseSample2 * 0.5 * waveSize + noiseSample3 * 0.5 * waveSize; //+ float2(sin(uv.x +scaledTime*10), cos(uv.y+scaledTime*10)); //+ 0.1*float2(sin(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)),cos(uv.x * sin(waveAngle) + uv.y * cos(waveAngle)));

    float noiseSample4 = gnoise(waveMaskUV * scale * 16);

    float waveMask = 1 - ((waveMaskUV.x * sin(waveAngle) + waveMaskUV.y * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;

    float waveMaskoffset1 = 1 - (((waveMaskUV.x + 0.04 / scale) * sin(waveAngle) + (waveMaskUV.y + 0.04 / scale) * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;
    float waveMaskoffset2 = 1 - (((waveMaskUV.x + 0.08 / scale) * sin(waveAngle) + (waveMaskUV.y + 0.08 / scale) * cos(waveAngle) + noiseSample3 * 0.1 * waveSize) % (1 / scale)) * scale;
    float beforeNoise = waveMask + waveMaskoffset1 + waveMaskoffset2;

    waveMask = pow(waveMask, 2);
    float vorn = voronoi(uv*scale);
    float4 diffuse = tex2D(diffuseSampler, (waveMaskUV*scale + scaledTime * 8) % 1);

    waveMask *= vorn;

    waveMask += pow(waveMaskoffset1, 2);
    waveMask += pow(waveMaskoffset2, 2);

    waveMask *= pow(noiseSample, 1);
    //waveMask += pow(noiseSample4, 1)*waveMask*0.1;
    //waveMask *= pow(noiseSample4, 1) ;
    waveMask += pow(vorn, 4) * 0.1;

    //waveMask *= 2;
    
    waveMask = min(waveMask, 1);

    //waveMask = floor(waveMask * 16) / 16;
    
    float3 waveColor = min(pow(beforeNoise / 2.5, 8) * min(diffuse.r+0.8, 1)*0.5 + (min(pow(beforeNoise / 2, 1.5), 1)) * diffuse.r * 1.5 + pow(waveMask + 0.3, 0.3) * float3(0.047, 0.086, 0.12) * 4, float3(1, 1, 1)); //should be all float(1,1,1) beforeNoise realism

    //waveMask = smoothstep(0.4,1,waveMask);
    
    
    //return float4(waveColor * lightColor, 1) * input.Color; //a is alpha right?
    return float4(waveColor* lightColor, 1);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

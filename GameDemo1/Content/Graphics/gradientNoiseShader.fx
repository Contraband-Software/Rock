#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

//Texture2D SpriteTexture;

//sampler2D SpriteTextureSampler = sampler_state
//{
//	Texture = <SpriteTexture>;
//};
float time;

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
    float2 uv = input.TextureCoordinates;
	//float4 sample =  tex2D(SpriteTextureSampler,uv);
    float noise = gnoise(uv * 8 + float2(time, time)*0.1);
    
    return float4(noise,noise,noise, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

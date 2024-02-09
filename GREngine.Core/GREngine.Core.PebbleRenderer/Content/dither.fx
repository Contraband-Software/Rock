#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler SpriteTextureSampler : register(s0);
sampler ditherSampler : register(s1);

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float2 ditherSize = float2(8., 8.);
float3 RGB_CHANNELS = float3(10, 10, 10);
float dither_f(float x, float c)
{
    x = min(x, 0.999);
    return floor(c * x) / c + 1. / (2. * c);
}
float3 dither(float3 color, float2 pixCoord)
{
    float3 col = color;
    float3 col2 = color;
    
    
    float3 col3 = float3(0,0,0);
    col3.r = dither_f(col.r, RGB_CHANNELS.r - 1.);
    col3.g = dither_f(col.g, RGB_CHANNELS.g - 1.);
    col3.b = dither_f(col.b, RGB_CHANNELS.b - 1.);

    col2 = col3 - 1. / (2. * (RGB_CHANNELS - 1.));
    col = col3 + 1. / (2. * (RGB_CHANNELS - 1.));
    
    
    float lerpR = (color.r - col.r) / (col2.r - col.r);
    float lerpG = (color.g - col.g) / (col2.g - col.g);
    float lerpB = (color.b - col.b) / (col2.b - col.b);

    float2 ditherCoordinate = pixCoord / ditherSize;
    
    float ditherval = tex2D(ditherSampler, ditherCoordinate).r;
    
    float3 ditheredval = float3(step(ditherval, lerpR), step(ditherval, lerpG), step(ditherval, lerpB));
    
    col = lerp(col, col2, ditheredval);
    
    return col;
}



float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    return float4(dither(col.rgb, input.TextureCoordinates),1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
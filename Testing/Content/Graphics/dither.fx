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



float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float4 col = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    return float4(floor(col.rgb * 16) / 16, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

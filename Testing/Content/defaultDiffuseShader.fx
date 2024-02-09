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

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 ligtmapUV = float2(input.Position.x / width, input.Position.y / height);
    float3 lightColor = tex2D(lightMap, ligtmapUV).rgb + ambientColor; //baked in ambient todo make it a parameter
	
    float2 uv = input.TextureCoordinates;
    float4 sample = tex2D(diffuseSampler, uv);
    return float4(sample * lightColor, sample.a) * input.Color; //a is alpha right?
	//+ (max(dot(lightColor, lightColor) - 2, 0) * lightColor * 0.2) //crude hdr
	
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
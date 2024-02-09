#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler SpriteTextureSampler : register(s0);


struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 sample = tex2D(SpriteTextureSampler, input.TextureCoordinates);
	
    float exposure = 2;
    float gamma = 2.2;
	
    //sample.rgb = max(0, sample.rgb - 0.004);
    //sample.rgb = (sample.rgb * (6.2 * sample.rgb + .5)) / (sample.rgb * (6.2 * sample.rgb + 1.7) + 0.06);
	
    //sample.rgb *= exposure / (1. + sample.rgb / exposure);
    //sample.rgb = pow(sample.rgb, 1. / gamma) ;
	
    float A = 0.15;
    float B = 0.50;
    float C = 0.10;
    float D = 0.20;
    float E = 0.02;
    float F = 0.30;
    float W = 11.2;
    sample.rgb *= exposure;
    sample.rgb = ((sample.rgb * (A * sample.rgb + C * B) + D * E) / (sample.rgb * (A * sample.rgb + B) + D * F)) - E / F;
    float white = ((W * (A * W + C * B) + D * E) / (W * (A * W + B) + D * F)) - E / F;
    sample.rgb /= white;
    sample.rgb = pow(sample.rgb, 1. / gamma);
    
    //sample.rgb = pow(sample.rgb, 0.85);
	
    return float4(sample.rgb, 1.0) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
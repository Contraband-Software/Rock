#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float time;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float2x2 rotationMatrix(float angle)
{
    float sine = sin(angle), cosine = cos(angle);
    return float2x2(cosine, -sine,
                 sine, cosine);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float pulseSpeed = 12;
    float rotationSpeed = 1;

    float2 uv = input.TextureCoordinates;
	float4 sample =  tex2D(SpriteTextureSampler,uv) * input.Color; //remember input.col\ifadsjiogr

    float alpha = sample.a;

    float len = length(abs(uv-0.5));

    float fresnel = len + sin(len * 16 + time * pulseSpeed) * 0.1;

    float sinr = sin(time * rotationSpeed);
    float cosr = cos(time * rotationSpeed);

    float2 rotatedUV = mul((uv - 0.5)*0.9, rotationMatrix(time * rotationSpeed)) + 0.5;
    float4 noiseSample =  tex2D(SpriteTextureSampler, rotatedUV);

    fresnel += (noiseSample / len) * 0.2 + pow((fresnel) % 0.5, 8) * 10;

    alpha = pow(step(0.2, max(fresnel - len, 0)) * step(len, 0.4 + fresnel * 0.1), 30);
    //return float4(noiseSample.rgb*sample.a, sample.a);

    fresnel = floor(fresnel * 8) / 8;

    float3 color = fresnel * float3(1, 0.6, 0.6) + alpha*0.2;

    return float4(color * alpha, alpha);


}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};

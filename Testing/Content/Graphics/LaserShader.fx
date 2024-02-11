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
sampler noiseMap : register(s1);

struct VertexShaderOutput
{
    float4 Position : SV_POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    
    float2 ssUV = float2(input.Position.x / width, input.Position.y / height);
	
    float2 uv = input.TextureCoordinates;

    //float2 uv = input.Position / float2(width, height);

    float4 noiseSample = tex2D(noiseMap, ssUV * 2 + time);

    float edge = length(float2(uv.x,uv.y) - 0.5);

    float beam = pow(1.4 - edge, 8) * pow(noiseSample.r, 0.1);

    
    return float4(float3(1, 0.6, 0.6)*beam, beam) * input.Color; //a is alpha right?
	
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

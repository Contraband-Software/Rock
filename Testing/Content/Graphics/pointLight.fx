#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define LIGHT_HEIGHT 100

float2 translation;
float time;
int width; //int?
int height;

float3 lightColor; //trying an approach rendering one light per shader -> maybe think about batching some light calls together?
float2 lightPosition;
float3 lightDirection; //x,y = light direction, z = arc

sampler normalSampler : register(s0);
sampler roughnessSampler : register(s1);
sampler shadowMapSampler : register(s2);




struct VertexShaderOutput
{
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float2 screenCords = float2(input.Position.x, input.Position.y); //does this still work?
	
    float3 normal = tex2D(normalSampler, uv).rgb * 2 - float3(1, 1, 1);
    float3 roughness = tex2D(roughnessSampler, uv).rgb;
    float shadow = tex2D(shadowMapSampler, uv).r;
    if (shadow == 0)
    {
        return float4(0, 0, 0, 1);
    }
	
    float2 translatedPos = lightPosition + translation;
    float3 distAxis = float3(screenCords, LIGHT_HEIGHT * 0.1) - float3(translatedPos, 0);
    float distSquared = distAxis.x * distAxis.x + distAxis.y * distAxis.y + LIGHT_HEIGHT * LIGHT_HEIGHT;
	
    float spotlightFactor = 1.0;
    if (lightDirection.z <1) //calculate if pixel is in spotlight cone
    {
       
        float spotTest = dot(normalize(distAxis), float3(lightDirection.xy, 0));
        float testVal = lightDirection.z; //- min(sqrt(distAxis.x * distAxis.x + distAxis.y * distAxis.y) * 0.1, 1); figure this out later

        if (spotTest < testVal)
        {
            return float4(0, 0, 0, 1);
        }
        else
        {
            spotlightFactor = max(min(pow(spotTest, abs(0 - testVal) * 4 / (1 - lightDirection.z)), 1.0), 0.0);
        }
        
    }
	
    //float shadingFactor = max(dot(normal.xyz, normalize(float3(-distAxis.x, distAxis.y, 100))), 0);
    float3 vecToCamera = normalize(float3(screenCords.x - width / 2, screenCords.y - height / 2, 10)); //-1000? is this right???
    
    float diffuseFactor = max(dot(normal, normalize(float3(distAxis.x, distAxis.y, LIGHT_HEIGHT))), 0.0);
    float glossyFactor = min(pow(dot(reflect(normalize(distAxis), normal), vecToCamera), 32), 1.0); //20?
    float shadingFactor = glossyFactor * roughness + diffuseFactor * (1 - roughness); // something still not quite right

	
    float3 lightingContribution = (lightColor / distSquared) * shadingFactor * spotlightFactor *shadow;
	
    return float4(lightingContribution, 1.0);
	
	
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define MAX_MARCH_STEPS 64
#define MARCH_STEP 8
#define LIGHT_HEIGHT 100


float2 translation;
float time;
int width; //int?
int height;

float3 lightColor; //trying an approach rendering one light per shader -> maybe think about batching some light calls together?
float2 lightPosition;
float3 lightDirection; //x,y = light direction, z = arc

//sampler lighingSampler : register(s0);
sampler occluderSampler : register(s0);




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

    float2 translatedPos = lightPosition + translation; //perhaps use this to optimise and realize when light is offscreen by a lot? // position of light in screenspace
    float3 distAxis = float3(screenCords, LIGHT_HEIGHT * 0.1) - float3(translatedPos, 0); //try values other than 0.1? //vector from  // vector from pixel/ shaded point to light
    float3 normalizedDistAxis = normalize(distAxis);
    float distSquared = distAxis.x * distAxis.x + distAxis.y * distAxis.y + LIGHT_HEIGHT * LIGHT_HEIGHT;


    if (lightDirection.z < 1) //calculate if pixel is in spotlight cone
    {
        float spotTest = dot(normalizedDistAxis, float3(lightDirection.xy, 0));
        float testVal = lightDirection.z; //- min(sqrt(distAxis.x * distAxis.x + distAxis.y * distAxis.y) * 0.1, 1); figure this out later

        if (spotTest < testVal)
        {
            return float4(0, 0, 0, 1);
        }
    }

    float shadowFactor = 1.0;
    float2 marchPos = screenCords; // is screencords right?
    float2 marchDir = normalize(translatedPos - screenCords) * MARCH_STEP; //see if i can just use normalizedDistAxis? should be opposite dir?

    //return tex2D(occluderSampler, screenCords / float2(width, height) + float2(0.5, 0.5));

    for (int i = 0; i < MAX_MARCH_STEPS; i++)
    {
        if (abs(translatedPos - marchPos).x < MARCH_STEP && abs(translatedPos - marchPos).y < MARCH_STEP)
        {
            break;
        }
        float sample = tex2D(occluderSampler, marchPos / float2(width*2, height*2) + float2(0.25, 0.25)).r; //see if I can sample without the div? //factor of two is multiplier for occlusiontexture buffer areas
        if (sample == 0)
        {
            shadowFactor -= 0.25;
        }
        if (shadowFactor <= 0)
        {
            return float4(0, 0, 0, 1);
        }
        marchPos += marchDir;
    }

    return float4(shadowFactor, shadowFactor, shadowFactor, 1.0);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};

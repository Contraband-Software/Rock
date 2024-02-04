#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


float2 translation;
float4x4 viewProjection;
float time;
float width;
float height;
float3 lightColors[64];
float2 lightPositions[64];
sampler colorSampler : register(s0);
sampler normalSampler : register(s1);
sampler occluderSampler : register(s2);
sampler unlitSampler : register(s3);

struct VertexInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};
struct PixelInput
{
    float4 Position : SV_Position0;
    float4 Color : COLOR0;
    float4 TexCoord : TEXCOORD0;
};


PixelInput SpriteVertexShader(VertexInput v)
{
    PixelInput output;

    output.Position = mul(v.Position, viewProjection);
    output.Color = v.Color;
    output.TexCoord = v.TexCoord;
    
    return output;
}
float4 SpritePixelShader(PixelInput p) : SV_TARGET
{
    float4 diffuse = tex2D(colorSampler, p.TexCoord.xy);
    float4 normal = tex2D(normalSampler, p.TexCoord.xy)*2 -1;
    float3 lighting = float3(1, 1, 1);
    float2 screenCords = float2(p.Position.x, p.Position.y);
    if (tex2D(unlitSampler, p.TexCoord.xy).r != 0)
    {
        lighting = float3(0.1, 0.1, 0.1);
        for (int i = 0; i < 64; i++)
        {
            if (dot(lightColors[i], float3(1, 1, 1)) > 0)
            {
                float2 translatedPos = lightPositions[i] + translation;
                float2 distAxis = screenCords - translatedPos;
                float distSquared = distAxis.x * distAxis.x + distAxis.y * distAxis.y;

                float2 marchVector = -normalize(distAxis) * 3; //-?
                float2 marchPos = screenCords;
                float3 shadowFactor = float3(1, 1, 1);

            //if (i == 0)
            //{
            //    for (int j = 0; j < 32; j++)
            //    {
            //        if (dot(shadowFactor, float3(1, 1, 1) <= 0))
            //        {
            //            shadowFactor = float3(0, 0, 0);
            //            break;
            //        }
            //        if (marchPos.x < 0 || marchPos.x > width || marchPos.y < 0 || marchPos.y > height)
            //        {
            //            break;
            //        }
            //        if (abs(translatedPos - marchPos).x < 3 && abs(translatedPos - marchPos).y < 3)
            //        {
            //            break;
            //        }
            //        float3 occluderSample = tex2D(occluderSampler, marchPos / float2(width, height)).rgb;
            //        if (dot(occluderSample, float3(1, 1, 1)) == 0)
            //        {
            //            shadowFactor -= float3(0.1, 0.1, 0.1);
            //        }
            //        marchPos += marchVector;
            //    }
            //}
                lighting += (lightColors[i] / distSquared) * max(dot(normal.xyz, normalize(float3(-distAxis.x, distAxis.y, 100))), 0) * shadowFactor; // not technically correct
            }
        }
    }
    return diffuse * float4(lighting, 1.0);
    //return float4(p.Position.x/width, p.Position.y/height, 0.0, 1.0);
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}

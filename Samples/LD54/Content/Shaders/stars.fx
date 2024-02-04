#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif


float4x4 viewProjection;
float time;
float width;
float height;
float strength;
float blackholeX;
float blackholeY;
float2 cpos;
sampler colorSampler : register(s0);

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
    float2 blackholePos = float2(blackholeX, blackholeY);
    float2 screenCords = float2(p.Position.x, p.Position.y);
    float2 diff = screenCords - blackholePos;
    float2 distortion = -normalize(diff) * (strength / (diff.x*diff.x+diff.y*diff.y));
    float2 texdistort = (p.TexCoord.xy) % 1 + distortion;
    float4 col = float4(0, 0.9, 1, 1); /*= tex2D(colorSampler, texdistort);*/

    float2 texparalax = texdistort + cpos * 0.0002;
    if (!((abs(texparalax.x % 0.02) < 0.0009) || (abs(texparalax.y % 0.02) < 0.0009)))
    {
        col = tex2D(colorSampler, texdistort);
    }
    
    return col;
}

technique SpriteBatch
{
    pass
    {
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
}

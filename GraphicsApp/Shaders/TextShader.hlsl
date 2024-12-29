//TextShader.hlsl
struct VSInput
{
    float3 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

struct PSInput
{
    float4 Position : SV_POSITION;
    float2 TexCoord : TEXCOORD0;
};

cbuffer Transform : register(b0)
{
    float4x4 WorldViewProjection;
};

PSInput VSMain(VSInput input)
{
    PSInput output;
    output.Position = mul(float4(input.Position, 1.0), WorldViewProjection);
    output.TexCoord = input.TexCoord;
    return output;
}

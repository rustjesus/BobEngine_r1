//LineVertexShader.hlsl
cbuffer ConstantBuffer : register(b0)
{
    float4x4 WorldViewProjection; // The combined World-View-Projection matrix
};

struct VSInput
{
    float3 Position : POSITION; // Input vertex position
    float4 Color : COLOR; // Input vertex color
};

struct PSInput
{
    float4 Position : SV_POSITION; // Transformed position
    float4 Color : COLOR; // Pass-through color
};

PSInput main(VSInput input)
{
    PSInput output;
    output.Position = mul(float4(input.Position, 1.0f), WorldViewProjection);
    output.Color = input.Color;
    return output;
}

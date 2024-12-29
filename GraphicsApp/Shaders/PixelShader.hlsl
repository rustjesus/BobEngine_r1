//PixelShader.hlsl
// Vertex shader
cbuffer MatrixBuffer : register(b0)
{
    float4x4 worldViewProj;
};

struct VertexInput
{
    float4 position : POSITION; // Matches Vertex.Position
    float2 texcoord : TEXCOORD; // Matches Vertex.TexCoord
};
struct PixelInput
{
    float4 position : SV_POSITION;
    float2 texcoord : TEXCOORD;
};

PixelInput VS(VertexInput input)
{
    PixelInput output;
    output.position = mul(input.position, worldViewProj);
    output.texcoord = input.texcoord;
    return output;
}

// Pixel shader
float4 PS(PixelInput input) : SV_TARGET
{
    return float4(1.0, 1.0, 1.0, 1.0); // White color
}

// Technique
technique11 Render
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, VS()));
        SetPixelShader(CompileShader(ps_5_0, PS()));
    }
}

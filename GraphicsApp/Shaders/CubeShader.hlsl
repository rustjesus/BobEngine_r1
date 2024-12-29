// CubeShader.hlsl
cbuffer MatrixBuffer : register(b0)
{
    matrix worldViewProjection;
};

struct VertexInput
{
    float3 position : POSITION;
    float4 color : COLOR; // Color attribute
};

struct PixelInput
{
    float4 position : SV_POSITION;
    float4 color : COLOR; // Color passed to the pixel shader
};

PixelInput VS(VertexInput input)
{
    PixelInput output;
    output.position = mul(float4(input.position, 1.0), worldViewProjection); // Apply WVP matrix
    output.color = input.color; // Pass color to the pixel shader
    return output;
}

float4 PS(PixelInput input) : SV_Target
{
    return input.color; // Output the color for each pixel
}

technique10 Render
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS()));
    }
}

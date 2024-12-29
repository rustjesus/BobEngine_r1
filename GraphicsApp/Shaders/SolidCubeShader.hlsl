cbuffer MatrixBuffer : register(b0)
{
    matrix worldViewProjection;
};

struct VertexInput
{
    float3 position : POSITION;
};

struct PixelInput
{
    float4 position : SV_POSITION;
};

PixelInput VS(VertexInput input)
{
    PixelInput output;
    output.position = mul(float4(input.position, 1.0), worldViewProjection); // Apply WVP matrix
    return output;
}

float4 PS(PixelInput input) : SV_Target
{
    return float4(0, 0, 0, 1.0); //change for different colors
}

technique10 Render
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS()));
    }
}
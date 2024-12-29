float4x4 WorldViewProjection;

struct VS_INPUT
{
    float4 Position : POSITION;
    float4 Color : COLOR;
};

struct VS_OUTPUT
{
    float4 Position : POSITION;
    float4 Color : COLOR;
};

VS_OUTPUT VS_Main(VS_INPUT input)
{
    VS_OUTPUT output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    return output;
}

float4 PS_Main(VS_OUTPUT input) : COLOR
{
    return input.Color;
}

technique RenderTechnique
{
    pass P0
    {
        VertexShader = compile vs_3_0 VS_Main();
        PixelShader = compile ps_3_0 PS_Main();
    }
}

//LinePixelShader.hlsl
struct PSInput
{
    float4 Position : SV_POSITION; // Interpolated position from the vertex shader
    float4 Color : COLOR; // Interpolated color from the vertex shader
};

float4 main(PSInput input) : SV_TARGET
{
    return input.Color; // Output the color to the render target
}


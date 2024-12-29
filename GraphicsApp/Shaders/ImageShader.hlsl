//ImageShader.hlsl
Texture2D shaderTexture : register(t0); // Texture resource
SamplerState sampleType : register(s0); // Sampler state

struct VertexInput
{
    float4 position : POSITION; // Vertex position
    float2 texcoord : TEXCOORD; // Texture coordinates
};

struct PixelInput
{
    float4 position : SV_POSITION;
    float2 texcoord : TEXCOORD;
};

PixelInput VS(VertexInput input)
{
    PixelInput output;
    output.position = input.position;
    output.texcoord = input.texcoord;
    return output;
}

float4 PS(PixelInput input) : SV_TARGET
{
    return shaderTexture.Sample(sampleType, input.texcoord);
}

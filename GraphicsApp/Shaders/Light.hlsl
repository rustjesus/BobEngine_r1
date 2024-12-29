// Vertex Shader (LightShader.hlsl)
cbuffer ConstantBuffer : register(b0)
{
    matrix worldViewProjection; // World * View * Projection matrix
    matrix worldMatrix; // World matrix
    float4 lightPosition; // Light position in world space
};

struct VertexInput
{
    float3 position : POSITION;
    float3 normal : NORMAL;
};

struct VertexOutput
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float3 fragPosition : TEXCOORD0;
};

VertexOutput VS(VertexInput input)
{
    VertexOutput output;

    // Transform vertex position to clip space
    output.position = mul(float4(input.position, 1.0f), worldViewProjection);
    
    // Calculate the position in world space for lighting
    output.fragPosition = mul(float4(input.position, 1.0f), worldMatrix).xyz;
    
    // Pass the normal for lighting calculation
    output.normal = input.normal;
    
    return output;
}

// Pixel Shader (LightShader.hlsl)
struct VertexOutput
{
    float4 position : SV_POSITION;
    float3 normal : NORMAL;
    float3 fragPosition : TEXCOORD0;
};

float4 PS(VertexOutput input) : SV_Target
{
    // Light direction (from the light position to the fragment position)
    float3 lightDir = normalize(lightPosition.xyz - input.fragPosition);
    
    // Basic Lambertian diffuse shading model
    float diffuse = max(dot(input.normal, lightDir), 0.0f);
    
    // Simple color
    float4 diffuseColor = float4(diffuse, diffuse, diffuse, 1.0f);
    
    return diffuseColor;
}

technique10 RenderLight
{
    pass P0
    {
        SetVertexShader(CompileShader(vs_5_0, VS()));
        SetPixelShader(CompileShader(ps_5_0, PS()));
    }
}

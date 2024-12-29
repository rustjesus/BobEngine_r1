using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.IO;
using System.Runtime.Remoting.Contexts;
using SharpDX.Direct3D9;

public class Shader
{
    private SharpDX.Direct3D11.Device device;
    public SharpDX.Direct3D11.VertexShader VertexShader { get; private set; }
    public SharpDX.Direct3D11.PixelShader PixelShader { get; private set; }
    public InputLayout InputLayout { get; private set; }
    private DeviceContext context;
    private string shaderPath;
    public Shader(SharpDX.Direct3D11.Device device, string shaderPath)
    {
        // Check if the shader file exists
        if (!File.Exists(shaderPath))
        {
            throw new IOException($"Shader file not found at: {shaderPath}");
        }

        // Compile vertex shader
        var vertexShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderPath, "VS", "vs_4_0");
        VertexShader = new SharpDX.Direct3D11.VertexShader(device, vertexShaderByteCode);

        // Compile pixel shader
        var pixelShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(shaderPath, "PS", "ps_4_0");
        PixelShader = new SharpDX.Direct3D11.PixelShader(device, pixelShaderByteCode);

        // Define the input layout based on the shader
        var inputElements = new[]
        {
            new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
            new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 12, 0)  // Match the color format
        };

        InputLayout = new InputLayout(device, ShaderSignature.GetInputSignature(vertexShaderByteCode), inputElements);
        this.shaderPath = shaderPath;
    }

    // Set the shadow map as a shader resource in the pixel shader
    public void SetShadowMap(ShaderResourceView shadowMap)
    {
        context.PixelShader.SetShaderResource(0, shadowMap);
    }

    // Set shaders for rendering
    public void SetShaders(DeviceContext context)
    {
        context.VertexShader.Set(VertexShader);
        context.PixelShader.Set(PixelShader);
        context.InputAssembler.InputLayout = InputLayout;
    }

    // Optionally, set constant buffers for the vertex shader if needed
    public void SetConstantBuffer(DeviceContext context, SharpDX.Direct3D11.Buffer constantBuffer)
    {
        context.VertexShader.SetConstantBuffer(0, constantBuffer);
    }
    public void SetWorldViewProjection(DeviceContext context, Matrix wvp)
    {
        // Assume you have a constant buffer for the WVP matrix
        var wvpBuffer = new SharpDX.Direct3D11.Buffer(device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);
        context.UpdateSubresource(ref wvp, wvpBuffer);
        context.VertexShader.SetConstantBuffer(0, wvpBuffer);
    }
}

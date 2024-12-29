using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

public class Shadow : IDisposable
{
    private SharpDX.Direct3D11.Device device;
    private int width;
    private int height;

    public DepthStencilView ShadowDepthView { get; private set; }
    public ShaderResourceView ShadowMap { get; private set; }
    public Viewport ShadowViewport { get; private set; }

    private Matrix lightViewProjectionMatrix;

    public Shadow(SharpDX.Direct3D11.Device device, int width, int height)
    {
        this.device = device;
        this.width = width;
        this.height = height;

        InitializeShadowResources();
    }

    private void InitializeShadowResources()
    {
        // Texture description for depth map
        var textureDesc = new Texture2DDescription
        {
            Width = width,
            Height = height,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.R32_Typeless,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.DepthStencil | BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        };

        // Create the texture for the shadow map
        using (var shadowMapTexture = new Texture2D(device, textureDesc))
        {
            // DepthStencilView description
            var depthViewDesc = new DepthStencilViewDescription
            {
                Dimension = DepthStencilViewDimension.Texture2D,
                Format = Format.D32_Float,
                Flags = DepthStencilViewFlags.None
            };
            ShadowDepthView = new DepthStencilView(device, shadowMapTexture, depthViewDesc);

            // ShaderResourceView description
            var srvDesc = new ShaderResourceViewDescription
            {
                Dimension = ShaderResourceViewDimension.Texture2D,
                Format = Format.R32_Float,
                Texture2D = new ShaderResourceViewDescription.Texture2DResource
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };
            ShadowMap = new ShaderResourceView(device, shadowMapTexture, srvDesc);
        }

        // Setup viewport for shadow map
        ShadowViewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
    }

    public void SetLightViewProjectionMatrix(Matrix matrix)
    {
        lightViewProjectionMatrix = matrix;
    }

    public Matrix GetLightViewProjectionMatrix()
    {
        return lightViewProjectionMatrix;
    }

    public void Dispose()
    {
        ShadowDepthView?.Dispose();
        ShadowMap?.Dispose();
    }
}

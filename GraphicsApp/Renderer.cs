using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D9;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using System;

public class Renderer
{
    private SharpDX.Direct3D11.Device device11;
    private SharpDX.DXGI.SwapChain swapChain;
    private DeviceContext context;
    private RenderTargetView renderTargetView;
    private DepthStencilView depthStencilView;

    public Renderer(SharpDX.Direct3D11.Device device, SharpDX.DXGI.SwapChain swapChain, DeviceContext context)
    {
        this.device11 = device;
        this.swapChain = swapChain;
        this.context = context;
    }

    public void InitializeRenderTarget(Viewport viewport)
    {
        using (var backBuffer = swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0))
        {
            renderTargetView?.Dispose();
            renderTargetView = new RenderTargetView(device11, backBuffer);
        }

        // Create a depth stencil buffer
        using (var depthBuffer = new Texture2D(device11, new Texture2DDescription
        {
            Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
            ArraySize = 1,
            MipLevels = 1,
            Width = (int)viewport.Width,
            Height = (int)viewport.Height,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.DepthStencil,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        }))
        {
            depthStencilView?.Dispose();
            depthStencilView = new DepthStencilView(device11, depthBuffer);
        }

        context.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
        context.Rasterizer.SetViewport(viewport);
    }

    public void Resize(int width, int height)
    {
        // Dispose the old views
        renderTargetView?.Dispose();
        depthStencilView?.Dispose();

        // Resize the swap chain buffers
        swapChain.ResizeBuffers(0, width, height, SharpDX.DXGI.Format.Unknown, SwapChainFlags.None);

        // Recreate the render target view
        using (var backBuffer = swapChain.GetBackBuffer<SharpDX.Direct3D11.Texture2D>(0))
        {
            renderTargetView = new RenderTargetView(device11, backBuffer);
        }

        // Recreate the depth stencil view
        using (var depthBuffer = new Texture2D(device11, new Texture2DDescription
        {
            Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
            ArraySize = 1,
            MipLevels = 1,
            Width = width,
            Height = height,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Default,
            BindFlags = BindFlags.DepthStencil,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        }))
        {
            depthStencilView = new DepthStencilView(device11, depthBuffer);
        }

        // Update the viewport
        var viewport = new Viewport(0, 0, width, height);
        context.Rasterizer.SetViewport(viewport);
        context.OutputMerger.SetRenderTargets(depthStencilView, renderTargetView);
    }

    public void ClearScreen(RawColor4 color)
    {
        context.ClearRenderTargetView(renderTargetView, color);
        context.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);
    }

    public void Present()
    {
        swapChain.Present(1, SharpDX.DXGI.PresentFlags.None);
    }
}

public class RendererDX9 : IDisposable
{
    private SharpDX.Direct3D9.Device device9;

    public RendererDX9(SharpDX.Direct3D9.Device device)
    {
        this.device9 = device;
    }

    public void InitializeRenderTarget(int width, int height)
    {
        // Set the viewport
        var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
        device9.Viewport = viewport;

        // Enable Z-buffering
        device9.SetRenderState(RenderState.ZEnable, true);
        device9.SetRenderState(RenderState.ZWriteEnable, true);
        device9.SetRenderState(RenderState.ZFunc, Compare.LessEqual);
    }

    public void Resize(int width, int height)
    {
        // Update the viewport with the new size
        var viewport = new Viewport(0, 0, width, height, 0.0f, 1.0f);
        device9.Viewport = viewport;
    }

    public void ClearScreen(RawColorBGRA color)
    {
        // Clear the render target and depth buffer
        device9.Clear(ClearFlags.Target | ClearFlags.ZBuffer, color, 1.0f, 0);
    }

    public void Present()
    {
        // Present the back buffer to the screen
        device9.Present();
    }

    public void Dispose()
    {
        // Release resources
        device9?.Dispose();
    }
}

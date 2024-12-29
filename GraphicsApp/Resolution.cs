using GraphicsApp;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Collections.Generic;
using SharpDX.Direct3D9;

internal class Resolution
{
    public int Width { get; }
    public int Height { get; }

    public Resolution(int width, int height)
    {
        Width = width;
        Height = height;
    }

    public override string ToString()
    {
        return $"{Width}x{Height}";
    }

    public void ApplyRes(Form1 form, Renderer renderer, Camera camera, DeviceContext context, SharpDX.DXGI.SwapChain swapChain)
    {
        form.ClientSize = new System.Drawing.Size(Width, Height);

        // Resize the renderer
        renderer.Resize(Width, Height);

        // Update the camera projection matrix
        float aspectRatio = Width / (float)Height;
        camera.UpdateProjection(aspectRatio);

        // Set the viewport
        var viewport = new Viewport(0, 0, Width, Height);
        context.Rasterizer.SetViewport(viewport);

        // Reinitialize the render target
        renderer.InitializeRenderTarget(viewport);

        // Ensure swap chain is presented after resize
        swapChain.Present(0, SharpDX.DXGI.PresentFlags.None);
    }
    public static void ChangeResolution(Form1 form, int resolutionIndex, List<Resolution> availableResolutions, Renderer renderer, Camera camera, DeviceContext context, SharpDX.DXGI.SwapChain swapChain)
    {
        if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Count)
            return;

        var newResolution = availableResolutions[resolutionIndex];
        newResolution.ApplyRes(form, renderer, camera, context, swapChain);

        // Log resolution change for debugging
        System.Diagnostics.Debug.WriteLine($"Resolution changed to: {newResolution.Width}x{newResolution.Height}");

        // Ensure the form regains focus
        form.Focus();
        form.Activate();
    }
}


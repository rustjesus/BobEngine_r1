using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace GraphicsApp
{
    internal class ResolutionDX9
    {
        public int Width { get; }
        public int Height { get; }

        public ResolutionDX9(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }

        public void ApplyResDX9(Form1 form, RendererDX9 renderer, Camera camera, SharpDX.Direct3D9.Device device)
        {
            // Adjust the form size
            form.ClientSize = new System.Drawing.Size(Width, Height);

            // Resize the renderer
            renderer.Resize(Width, Height);

            // Update the camera projection matrix
            float aspectRatio = Width / (float)Height;
            camera.UpdateProjection(aspectRatio);

            // Set the viewport
            var viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = Width,
                Height = Height,
            };
            device.Viewport = viewport;

            // Reinitialize the render target
            renderer.InitializeRenderTarget(Width, Height);

            // Log the resolution change
            System.Diagnostics.Debug.WriteLine($"Resolution applied: {Width}x{Height}");
        }
        public static void ChangeResolution(Form1 form, int resolutionIndex, List<ResolutionDX9> availableResolutions, RendererDX9 renderer, Camera camera, SharpDX.Direct3D9.Device device)
        {
            if (resolutionIndex < 0 || resolutionIndex >= availableResolutions.Count)
                return;

            var newResolution = availableResolutions[resolutionIndex];
            newResolution.ApplyResDX9(form, renderer, camera, device);

            // Log resolution change for debugging
            System.Diagnostics.Debug.WriteLine($"Resolution changed to: {newResolution.Width}x{newResolution.Height}");

            // Ensure the form regains focus
            form.Focus();
            form.Activate();
        }
    }

}

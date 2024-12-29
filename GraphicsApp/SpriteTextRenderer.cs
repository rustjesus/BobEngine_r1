using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.WIC;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GraphicsApp
{
    public class SpriteTextRenderer : IDisposable
    {
        private ShaderResourceView fontTextureView;
        private SamplerState samplerState;
        private SharpDX.Direct3D11.Device device;

        public SpriteTextRenderer(SharpDX.Direct3D11.Device device, string fontTexturePath)
        {
            this.device = device;
            LoadFontTexture(fontTexturePath);
            InitializeSamplerState();
        }

        private void LoadFontTexture(string path)
        {
            using (var factory = new ImagingFactory())
            using (var decoder = new BitmapDecoder(factory, path, DecodeOptions.CacheOnDemand))
            using (var frame = decoder.GetFrame(0))
            using (var converter = new FormatConverter(factory))
            {
                converter.Initialize(frame, PixelFormat.Format32bppRGBA);

                int stride = converter.Size.Width * 4;
                int bufferSize = stride * converter.Size.Height;
                IntPtr bufferPointer = Marshal.AllocHGlobal(bufferSize);

                try
                {
                    converter.CopyPixels(stride, bufferPointer, bufferSize);

                    var textureDesc = new Texture2DDescription
                    {
                        Width = converter.Size.Width,
                        Height = converter.Size.Height,
                        ArraySize = 1,
                        BindFlags = BindFlags.ShaderResource,
                        Usage = ResourceUsage.Immutable,
                        CpuAccessFlags = CpuAccessFlags.None,
                        Format = Format.R8G8B8A8_UNorm,
                        MipLevels = 1,
                        OptionFlags = ResourceOptionFlags.None,
                        SampleDescription = new SampleDescription(1, 0)
                    };

                    var dataRectangle = new DataRectangle(bufferPointer, stride);

                    using (var texture = new Texture2D(device, textureDesc, dataRectangle))
                    {
                        fontTextureView = new ShaderResourceView(device, texture);
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal(bufferPointer);
                }
            }
        }

        private void InitializeSamplerState()
        {
            var samplerDesc = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp
            };
            samplerState = new SamplerState(device, samplerDesc);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Vertex
        {
            public Vector3 Position;
            public Vector2 TexCoord;

            public Vertex(Vector3 position, Vector2 texCoord)
            {
                Position = position;
                TexCoord = texCoord;
            }
        }

        public void DrawTexture(DeviceContext context, Vector2 position, Vector2 size, ShaderResourceView previousShaderResource, SamplerState previousSampler)
        {
            // Save the current pixel shader resource and sampler (passed in by the caller)
            var oldShaderResource = previousShaderResource;
            var oldSamplerState = previousSampler;

            // Define vertices for the quad
            var vertices = new[]
            {
        new Vertex(new Vector3(position.X, position.Y, 0.0f), new Vector2(0.0f, 0.0f)),
        new Vertex(new Vector3(position.X + size.X, position.Y, 0.0f), new Vector2(1.0f, 0.0f)),
        new Vertex(new Vector3(position.X, position.Y + size.Y, 0.0f), new Vector2(0.0f, 1.0f)),
        new Vertex(new Vector3(position.X + size.X, position.Y + size.Y, 0.0f), new Vector2(1.0f, 1.0f))
    };

            using (var vertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices))
            {
                context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vertex>(), 0));

                // Bind the texture and sampler for rendering
                context.PixelShader.SetShaderResource(0, fontTextureView);
                context.PixelShader.SetSampler(0, samplerState);

                // Draw the quad
                context.Draw(4, 0);
            }

            // Restore the previous pixel shader resource and sampler
            context.PixelShader.SetShaderResource(0, oldShaderResource);
            context.PixelShader.SetSampler(0, oldSamplerState);
        }

        public void DrawText(DeviceContext context, Vector2 position, string text, RawColor4 color)
        {
            if (string.IsNullOrEmpty(text) || fontTextureView == null)
            {
                Console.WriteLine("Cannot draw text: either the text is empty or the font texture is not loaded.");
                return;
            }

            float x = position.X;
            float y = position.Y;
            float charWidth = 8;  // Width of a single character in your texture atlas
            float charHeight = 16; // Height of a single character in your texture atlas

            foreach (char c in text)
            {
                if (c == '\n') // Handle new lines
                {
                    x = position.X;
                    y += charHeight;
                    continue;
                }

                var charIndex = c - 32; // Assuming ASCII offset starts at space (' ')
                if (charIndex < 0 || charIndex >= 96) continue;

                var texCoordX = (charIndex % 16) / 16.0f; // Assuming 16x6 grid of characters
                var texCoordY = (charIndex / 16) / 6.0f;
                var texCoordSize = new Vector2(1.0f / 16.0f, 1.0f / 6.0f);

                var vertices = new[]
                {
            new Vertex(new Vector3(x, y, 0.0f), new Vector2(texCoordX, texCoordY)),
            new Vertex(new Vector3(x + charWidth, y, 0.0f), new Vector2(texCoordX + texCoordSize.X, texCoordY)),
            new Vertex(new Vector3(x, y + charHeight, 0.0f), new Vector2(texCoordX, texCoordY + texCoordSize.Y)),
            new Vertex(new Vector3(x + charWidth, y + charHeight, 0.0f), new Vector2(texCoordX + texCoordSize.X, texCoordY + texCoordSize.Y))
        };

                using (var vertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices))
                {
                    context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
                    context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vertex>(), 0));

                    context.PixelShader.SetShaderResource(0, fontTextureView);
                    context.PixelShader.SetSampler(0, samplerState);

                    context.Draw(4, 0);
                }

                x += charWidth; // Move to the next character position
            }
        }



        public void Dispose()
        {
            fontTextureView?.Dispose();
            samplerState?.Dispose();
        }
    }
}

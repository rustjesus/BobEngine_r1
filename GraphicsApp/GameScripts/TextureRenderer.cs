using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;
using System;
using Device = SharpDX.Direct3D11.Device;

namespace GraphicsApp.GameScripts
{
    internal class TextureRenderer
    {
        private ShaderResourceView textureView;
        private SamplerState samplerState;
        private VertexBufferBinding vertexBufferBinding;

        public void LoadTexture(Device device, string filePath)
        {
            try
            {
                // Use WIC (Windows Imaging Component) to load the PNG file
                ImagingFactory2 factory = new ImagingFactory2();
                BitmapDecoder decoder = new BitmapDecoder(factory, filePath, DecodeOptions.CacheOnDemand);
                BitmapFrameDecode frame = decoder.GetFrame(0);
                FormatConverter converter = new FormatConverter(factory);

                converter.Initialize(frame, SharpDX.WIC.PixelFormat.Format32bppRGBA);

                // Create texture from the WIC bitmap
                var textureDesc = new Texture2DDescription
                {
                    Width = converter.Size.Width,
                    Height = converter.Size.Height,
                    ArraySize = 1,
                    MipLevels = 1,
                    Format = Format.R8G8B8A8_UNorm,
                    SampleDescription = new SampleDescription(1, 0),
                    Usage = ResourceUsage.Immutable,
                    BindFlags = BindFlags.ShaderResource,
                    CpuAccessFlags = CpuAccessFlags.None,
                    OptionFlags = ResourceOptionFlags.None
                };

                var pixelData = new DataStream(converter.Size.Width * converter.Size.Height * 4, true, true);
                converter.CopyPixels(converter.Size.Width * 4, pixelData);
                pixelData.Position = 0;

                var texture = new Texture2D(device, textureDesc, new DataRectangle(pixelData.DataPointer, converter.Size.Width * 4));

                // Create a ShaderResourceView for the texture
                textureView = new ShaderResourceView(device, texture);

                // Clean up WIC resources
                pixelData.Dispose();
                converter.Dispose();
                frame.Dispose();
                decoder.Dispose();
                factory.Dispose();

                // Create a sampler state
                samplerState = new SamplerState(device, new SamplerStateDescription
                {
                    Filter = Filter.MinMagMipLinear,
                    AddressU = TextureAddressMode.Clamp,
                    AddressV = TextureAddressMode.Clamp,
                    AddressW = TextureAddressMode.Clamp,
                    BorderColor = new Color4(0, 0, 0, 1),
                    ComparisonFunction = Comparison.Always,
                    MaximumLod = float.MaxValue,
                    MinimumLod = 0,
                    MipLodBias = 0
                });

                Console.WriteLine("Texture loaded successfully from: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load texture: " + ex.Message);
            }
        }

        public void Render(DeviceContext context, Matrix viewProjectionMatrix, Matrix worldMatrix)
        {
            if (textureView == null || samplerState == null)
            {
                Console.WriteLine("Texture or sampler state not initialized.");
                return;
            }

            // Set the shader resource and sampler state
            context.PixelShader.SetShaderResource(0, textureView);
            context.PixelShader.SetSampler(0, samplerState);

            // Define vertices for a quad
            var vertices = new[]
            {
                new Vertex { Position = new Vector3(-0.5f, 0.5f, 0), UV = new Vector2(0, 0) },
                new Vertex { Position = new Vector3(0.5f, 0.5f, 0), UV = new Vector2(1, 0) },
                new Vertex { Position = new Vector3(-0.5f, -0.5f, 0), UV = new Vector2(0, 1) },
                new Vertex { Position = new Vector3(0.5f, -0.5f, 0), UV = new Vector2(1, 1) },
            };

            // Create vertex buffer
            var vertexBuffer = SharpDX.Direct3D11.Buffer.Create(context.Device, BindFlags.VertexBuffer, vertices);
            vertexBufferBinding = new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<Vertex>(), 0);

            // Apply the vertex buffer
            context.InputAssembler.SetVertexBuffers(0, vertexBufferBinding);
            context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleStrip;

            // Set transformation matrices
            var worldViewProjection = Matrix.Transpose(worldMatrix * viewProjectionMatrix);
            context.VertexShader.SetConstantBuffer(0, SharpDX.Direct3D11.Buffer.Create(context.Device, BindFlags.ConstantBuffer, ref worldViewProjection));

            // Draw the quad
            context.Draw(4, 0);
        }

        public void Dispose()
        {
            textureView?.Dispose();
            samplerState?.Dispose();
        }

        private struct Vertex
        {
            public Vector3 Position;
            public Vector2 UV;
        }
    }
}

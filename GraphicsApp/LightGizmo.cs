using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;

namespace GraphicsApp
{
    public struct VertexPositionColor
    {
        public Vector3 Position;
        public Color4 Color;

        public VertexPositionColor(Vector3 position, Color4 color)
        {
            Position = position;
            Color = color;
        }
    }

    public class LightGizmo
    {
        private SharpDX.Direct3D11.Device device;
        private SharpDX.Direct3D11.Buffer vertexBuffer;
        private SharpDX.Direct3D11.Buffer indexBuffer;
        private Shader shader;
        private Matrix worldMatrix;

        public LightGizmo(SharpDX.Direct3D11.Device device, string shaderPath)
        {
            this.device = device;

            // Create a simple sphere or cube geometry for the gizmo
            CreateGeometry();

            // Load and set the shader
            //shader = new Shader(device, shaderPath);
        }

        private void CreateGeometry()
        {
            // Define a small cube around the light position
            var vertices = new[]
            {
        new VertexPositionColor(new Vector3(-0.1f, -0.1f, -0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3( 0.1f, -0.1f, -0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3( 0.1f,  0.1f, -0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(-0.1f,  0.1f, -0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(-0.1f, -0.1f,  0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3( 0.1f, -0.1f,  0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3( 0.1f,  0.1f,  0.1f), new Color4(1, 0, 0, 1)),
        new VertexPositionColor(new Vector3(-0.1f,  0.1f,  0.1f), new Color4(1, 0, 0, 1)),
    };

            var indices = new[]
            {
        0, 1, 2, 2, 3, 0, // Front
        4, 5, 6, 6, 7, 4, // Back
        0, 1, 5, 5, 4, 0, // Left
        1, 2, 6, 6, 5, 1, // Right
        2, 3, 7, 7, 6, 2, // Top
        3, 0, 4, 4, 7, 3  // Bottom
    };

            // Create buffers for the geometry
            using (var dataStream = new DataStream(vertices.Length * Utilities.SizeOf<VertexPositionColor>(), true, true))
            {
                // Copy the vertex data into the data stream
                dataStream.WriteRange(vertices);

                // Now create the buffer
                vertexBuffer = new SharpDX.Direct3D11.Buffer(device, dataStream, new BufferDescription
                {
                    SizeInBytes = vertices.Length * Utilities.SizeOf<VertexPositionColor>(), // Size in bytes
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.VertexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None
                });
            }


            using (var dataStream = new DataStream(indices.Length * Utilities.SizeOf<int>(), true, true))
            {
                // Copy the index data into the data stream
                dataStream.WriteRange(indices);

                // Now create the index buffer
                indexBuffer = new SharpDX.Direct3D11.Buffer(device, dataStream, new BufferDescription
                {
                    SizeInBytes = indices.Length * Utilities.SizeOf<int>(), // Size in bytes
                    Usage = ResourceUsage.Default,
                    BindFlags = BindFlags.IndexBuffer,
                    CpuAccessFlags = CpuAccessFlags.None
                });
            }

        }

        /*
        public void Render(DeviceContext context, Matrix viewProjection, Matrix worldMatrix)
        {
            this.worldMatrix = worldMatrix;

            // Set the shader and buffers
            shader.SetShaders(context);
            context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            // Set the vertex buffer
            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));
            context.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

            // Set the world matrix and view-projection
            var wvp = worldMatrix * viewProjection;
            shader.SetWorldViewProjection(context, wvp);

            // Draw the gizmo (a cube)
            context.DrawIndexed(36, 0, 0); // 36 indices (6 faces * 2 triangles * 3 vertices)
        }*/
    }
}

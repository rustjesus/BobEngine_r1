using System;
using SharpDX.Direct3D9;
using SharpDX;

namespace GraphicsApp
{
    internal class CubeDX9 : IDisposable
    {
        private Device device;
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private bool buffersLoaded = false;
        public bool isDestroyed { get; set; }
        public bool isDestroyedHandled = false;

        // Vertex structure with position and color
        struct Vertex
        {
            public Vector3 Position;
            public int Color;

            public Vertex(Vector3 position, int color)
            {
                Position = position;
                Color = color;
            }
        }

        public CubeDX9(Device device)
        {
            if (!buffersLoaded)
            {
                this.device = device;
                InitializeBuffers();
                buffersLoaded = true;
            }
        }

        private void InitializeBuffers()
        {
            // Define vertices with color for each side
            var vertices = new[]
            {
                new Vertex(new Vector3(-1.0f, -1.0f,  1.0f), unchecked((int)0xFFFF0000)), // Front face (red)
                new Vertex(new Vector3( 1.0f, -1.0f,  1.0f), unchecked((int)0xFFFF0000)),
                new Vertex(new Vector3( 1.0f,  1.0f,  1.0f), unchecked((int)0xFFFF0000)),
                new Vertex(new Vector3(-1.0f,  1.0f,  1.0f), unchecked((int)0xFFFF0000)),
                new Vertex(new Vector3(-1.0f, -1.0f, -1.0f), unchecked((int)0xFF00FF00)), // Back face (green)
                new Vertex(new Vector3(-1.0f,  1.0f, -1.0f), unchecked((int)0xFF00FF00)),
                new Vertex(new Vector3( 1.0f,  1.0f, -1.0f), unchecked((int)0xFF00FF00)),
                new Vertex(new Vector3( 1.0f, -1.0f, -1.0f), unchecked((int)0xFF00FF00)),
            };

            // Create vertex buffer
            vertexBuffer = new VertexBuffer(
                device,
                Utilities.SizeOf<Vertex>() * vertices.Length,
                Usage.WriteOnly,
                VertexFormat.Position | VertexFormat.Diffuse,
                Pool.Default);

            // Write vertices to the buffer
            using (var stream = vertexBuffer.Lock(0, 0, LockFlags.None))
            {
                stream.WriteRange(vertices);
            }
            vertexBuffer.Unlock();

            // Define indices for the cube
            var indices = new short[]
            {
                0, 1, 2, 0, 2, 3, // Front face
                4, 5, 6, 4, 6, 7, // Back face
                0, 3, 5, 0, 5, 4, // Left face
                1, 7, 6, 1, 6, 2, // Right face
                3, 2, 6, 3, 6, 5, // Top face
                0, 4, 7, 0, 7, 1  // Bottom face
            };

            // Create index buffer
            indexBuffer = new IndexBuffer(
                device,
                sizeof(short) * indices.Length,
                Usage.WriteOnly,
                Pool.Default,
                true);

            // Write indices to the buffer
            using (var stream = indexBuffer.Lock(0, 0, LockFlags.None))
            {
                stream.WriteRange(indices);
            }
            indexBuffer.Unlock();
        }

        public void Render()
        {
            // Skip rendering if destroyed
            if (isDestroyed || vertexBuffer == null || indexBuffer == null)
                return;

            // Set buffers
            device.SetStreamSource(0, vertexBuffer, 0, Utilities.SizeOf<Vertex>());
            device.Indices = indexBuffer;
            device.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;

            // Draw the cube
            device.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
        }

        public void Destroy()
        {
            if (!isDestroyed)
            {
                // Dispose buffers
                vertexBuffer?.Dispose();
                indexBuffer?.Dispose();

                vertexBuffer = null;
                indexBuffer = null;

                isDestroyed = true;

                Console.WriteLine("Cube destroyed.");
            }
        }

        public void Dispose()
        {
            Destroy();
        }
    }
}

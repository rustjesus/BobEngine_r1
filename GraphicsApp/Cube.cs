using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.D3DCompiler;
using System;
using SharpDX.Direct3D9;
using SharpGL.VertexBuffers;

namespace GraphicsApp
{
    public class Cube
    {
        public float cubeSize = 0.1f;
        // Direct3D11 resources
        private SharpDX.Direct3D11.Device device11;
        private SharpDX.Direct3D11.Buffer vertexBuffer11;
        private SharpDX.Direct3D11.Buffer indexBuffer11;
        private SharpDX.Direct3D11.Buffer constantBuffer11;

        // Direct3D9 resources
        private SharpDX.Direct3D9.Device device9;
        private SharpDX.Direct3D9.VertexBuffer vertexBuffer9;
        private SharpDX.Direct3D9.IndexBuffer indexBuffer9;


        private bool buffersLoaded = false;
        public bool isDestroyed { get; set; }
        public bool isDestroyedHandled = false;


        // Vertex structure with position, color, and normal
        struct Vertex
        {
            public Vector3 Position;
            public Color4 Color;
            public Vector3 Normal;

            public Vertex(Vector3 position, Color4 color, Vector3 normal)
            {
                Position = position;
                Color = color;
                Normal = normal;
            }
        }

        // Constructor for Direct3D11
        public Cube(SharpDX.Direct3D9.Device device)
        {
            device9 = device;
            InitializeBuffersDX9(); // Ensure only DX9 buffers are initialized
        }

        public Cube(SharpDX.Direct3D11.Device device)
        {
            device11 = device;
            if(Form1.usingDirectx11)
            {
                InitializeBuffers(); // Ensure only DX11 buffers are initialized

            }
        }

        public void InitializeBuffers()
        {
            // Define vertices with color and normal for each side
            var vertices = new[]
            {
                // Front face (red) - Normal (0, 0, 1)
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),

                // Back face (green) - Normal (0, 0, -1)
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),

                // Top face (blue) - Normal (0, 1, 0)
                new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),

                // Bottom face (yellow) - Normal (0, -1, 0)
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),

                // Left face (purple) - Normal (-1, 0, 0)
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),

                // Right face (orange) - Normal (1, 0, 0)
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
            };


            // Define indices (same as before)
            // Define indices (corrected for outward-facing normals)
            var indices = new ushort[]
            {
                0, 1, 2, 0, 2, 3,  // Front face
                4, 5, 6, 4, 6, 7,  // Back face
                8, 9, 10, 8, 10, 11, // Top face
                12, 13, 14, 12, 14, 15, // Bottom face
                16, 17, 18, 16, 18, 19, // Left face
                20, 21, 22, 20, 22, 23  // Right face
            };

            vertexBuffer11 = SharpDX.Direct3D11.Buffer.Create(device11, BindFlags.VertexBuffer, vertices);
            indexBuffer11 = SharpDX.Direct3D11.Buffer.Create(device11, BindFlags.IndexBuffer, indices);

            constantBuffer11 = SharpDX.Direct3D11.Buffer.Create(device11, BindFlags.ConstantBuffer, new Matrix[] { Matrix.Identity });
            if (Form1.usingDirectx11)
            {

            }
            /*
            if (Form1.usingDirectx9)
            {
                // Dispose the old buffer if it exists
                vertexBuffer9?.Dispose();

                // Create new vertex buffer for DX9
                vertexBuffer9 = new SharpDX.Direct3D9.VertexBuffer(
                    device9,
                    Utilities.SizeOf<Vertex>() * vertices.Length,
                    Usage.WriteOnly,
                    VertexFormat.Position | VertexFormat.Diffuse,
                    Pool.Default
                );

                // Lock the buffer and write the new vertices
                using (var stream = vertexBuffer9.Lock(0, 0, LockFlags.None))
                {
                    stream.WriteRange(vertices);
                    vertexBuffer9.Unlock();
                }
                indexBuffer9 = new SharpDX.Direct3D9.IndexBuffer(
                    device9,
                    sizeof(ushort) * indices.Length,
                    Usage.WriteOnly,
                    Pool.Default,
                    true
                );

                using (var stream = indexBuffer9.Lock(0, 0, LockFlags.None))
                {
                    stream.WriteRange(indices);
                    indexBuffer9.Unlock();
                }
            }*/
        }
        private void InitializeBuffersDX9()
        {
            // Define vertices with color for each side
            var vertices = new[]
            {
                // Front face (red) - Normal (0, 0, 1)
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                new Vertex(new Vector3(-cubeSize, cubeSize,  cubeSize), new Color4(1.0f, 0.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),

                // Back face (green) - Normal (0, 0, -1)
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3(-cubeSize, cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(0.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),

                // Top face (blue) - Normal (0, 1, 0)
                new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(0.0f, 0.0f, 1.0f, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),

                // Bottom face (yellow) - Normal (0, -1, 0)
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 1.0f, 0.0f, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),

                // Left face (purple) - Normal (-1, 0, 0)
                new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(0.5f, 0.0f, 0.5f, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),

                // Right face (orange) - Normal (1, 0, 0)
                new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(1.0f, 0.5f, 0.0f, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
            };

            // Create vertex buffer
            vertexBuffer9 = new SharpDX.Direct3D9.VertexBuffer(
                device9,
                Utilities.SizeOf<Vertex>() * vertices.Length,
                Usage.WriteOnly,
                VertexFormat.Position | VertexFormat.Diffuse,
                Pool.Default);

            // Write vertices to the buffer
            using (var stream = vertexBuffer9.Lock(0, 0, LockFlags.None))
            {
                stream.WriteRange(vertices);
            }
            vertexBuffer9.Unlock();

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
            indexBuffer9 = new SharpDX.Direct3D9.IndexBuffer(
                device9,
                sizeof(short) * indices.Length,
                Usage.WriteOnly,
                Pool.Default,
                true);

            // Write indices to the buffer
            using (var stream = indexBuffer9.Lock(0, 0, LockFlags.None))
            {
                stream.WriteRange(indices);
            }
            indexBuffer9.Unlock();
        }
        public void SetFaceColors(Vector3[] faceColors)
        {

                // Update vertices with new colors
                var vertices = new[]
                {
                    // Front face (faceColors[0])
                    new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[0].X, faceColors[0].Y, faceColors[0].Z, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                    new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[0].X, faceColors[0].Y, faceColors[0].Z, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                    new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[0].X, faceColors[0].Y, faceColors[0].Z, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),
                    new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[0].X, faceColors[0].Y, faceColors[0].Z, 1.0f), new Vector3(0.0f, 0.0f, cubeSize)),

                    // Back face (faceColors[1])
                    new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[1].X, faceColors[1].Y, faceColors[1].Z, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                    new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[1].X, faceColors[1].Y, faceColors[1].Z, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                    new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[1].X, faceColors[1].Y, faceColors[1].Z, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),
                    new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[1].X, faceColors[1].Y, faceColors[1].Z, 1.0f), new Vector3(0.0f, 0.0f, -cubeSize)),

                    // Top face (faceColors[2])
                    new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[2].X, faceColors[2].Y, faceColors[2].Z, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                    new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[2].X, faceColors[2].Y, faceColors[2].Z, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                    new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[2].X, faceColors[2].Y, faceColors[2].Z, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),
                    new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[2].X, faceColors[2].Y, faceColors[2].Z, 1.0f), new Vector3(0.0f, cubeSize, 0.0f)),

                    // Bottom face (faceColors[3])
                    new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[3].X, faceColors[3].Y, faceColors[3].Z, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                    new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[3].X, faceColors[3].Y, faceColors[3].Z, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                    new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[3].X, faceColors[3].Y, faceColors[3].Z, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),
                    new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[3].X, faceColors[3].Y, faceColors[3].Z, 1.0f), new Vector3(0.0f, -cubeSize, 0.0f)),

                    // Left face (faceColors[4])
                    new Vertex(new Vector3(-cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[4].X, faceColors[4].Y, faceColors[4].Z, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3(-cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[4].X, faceColors[4].Y, faceColors[4].Z, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3(-cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[4].X, faceColors[4].Y, faceColors[4].Z, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3(-cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[4].X, faceColors[4].Y, faceColors[4].Z, 1.0f), new Vector3(-cubeSize, 0.0f, 0.0f)),

                    // Right face (faceColors[5])
                    new Vertex(new Vector3( cubeSize, -cubeSize,  cubeSize), new Color4(faceColors[5].X, faceColors[5].Y, faceColors[5].Z, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3( cubeSize, -cubeSize, -cubeSize), new Color4(faceColors[5].X, faceColors[5].Y, faceColors[5].Z, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3( cubeSize,  cubeSize, -cubeSize), new Color4(faceColors[5].X, faceColors[5].Y, faceColors[5].Z, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                    new Vertex(new Vector3( cubeSize,  cubeSize,  cubeSize), new Color4(faceColors[5].X, faceColors[5].Y, faceColors[5].Z, 1.0f), new Vector3(cubeSize, 0.0f, 0.0f)),
                };
            if(Form1.usingDirectx11)
            {

                vertexBuffer11?.Dispose(); // Dispose the old buffer
                vertexBuffer11 = SharpDX.Direct3D11.Buffer.Create(device11, BindFlags.VertexBuffer, vertices);
            }
            /*
            if(Form1.usingDirectx9)
            {
                // Dispose the old buffer
                vertexBuffer9?.Dispose();

                // Create new vertex buffer for DX9
                vertexBuffer9 = new SharpDX.Direct3D9.VertexBuffer(
                    device9,
                    Utilities.SizeOf<Vertex>() * vertices.Length,
                    Usage.WriteOnly,
                    VertexFormat.Position | VertexFormat.Diffuse,
                    Pool.Default
                );

                // Lock the buffer and write the new vertices
                using (var stream = vertexBuffer9.Lock(0, 0, LockFlags.None))
                {
                    stream.WriteRange(vertices);
                    vertexBuffer9.Unlock();
                }
            }*/
        }

        // Render for Direct3D11
        public void RenderDX11(DeviceContext context, Matrix worldViewProjection)
        {
            if (isDestroyed || vertexBuffer11 == null || indexBuffer11 == null)
                return;

            context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(vertexBuffer11, Utilities.SizeOf<Vertex>(), 0));
            context.InputAssembler.SetIndexBuffer(indexBuffer11, SharpDX.DXGI.Format.R16_UInt, 0);
            context.UpdateSubresource(ref worldViewProjection, constantBuffer11);
            context.VertexShader.SetConstantBuffer(0, constantBuffer11);

            context.DrawIndexed(36, 0, 0);
        }
        // Render for Direct3D9
        public void RenderDX9()
        {
            if (isDestroyed || vertexBuffer9 == null || indexBuffer9 == null)
                return;

            // Set the vertex buffer
            device9.SetStreamSource(0, vertexBuffer9, 0, Utilities.SizeOf<Vertex>());
            device9.Indices = indexBuffer9;

            // Specify the vertex format (Position and Color)
            device9.VertexFormat = VertexFormat.Position | VertexFormat.Diffuse;

            // Draw the indexed primitives
            device9.DrawIndexedPrimitive(PrimitiveType.TriangleList, 0, 0, 8, 0, 12);
        }


        public void Destroy()
        {
            if (!isDestroyed)
            {
                vertexBuffer11?.Dispose();
                indexBuffer11?.Dispose();
                constantBuffer11?.Dispose();

                vertexBuffer9?.Dispose();
                indexBuffer9?.Dispose();

                vertexBuffer11 = null;
                indexBuffer11 = null;
                constantBuffer11 = null;

                vertexBuffer9 = null;
                indexBuffer9 = null;


                isDestroyed = true;
                // Dispose resources
                this.Dispose();


                Console.WriteLine("Cube destroyed.");
            }
        }

        public void Dispose()
        {
            Destroy();
        }
    }
    public class CubeWithCollider : Cube
    {
        public Collider Collider { get; private set; }

        public Vector3 Velocity { get; set; }
        public Vector3 Position { get; private set; }
        public Matrix WorldMatrix { get; set; } = Matrix.Identity;


        public CubeWithCollider(SharpDX.Direct3D11.Device device, Vector3 position, float size, CollisionManager collisionManager)
            : base(device)
        {
            WorldMatrix = Matrix.Translation(position);
            cubeSize = size;
            Position = position;
            Vector3 halfSize = new Vector3(size / 2);
            Vector3 min = position - halfSize;
            Vector3 max = position + halfSize;

            Collider = new Collider(min, max);

            // Add the cube to the collision manager
            collisionManager.AddCube(this);
        }


        public void UpdateColliderPosition(Vector3 position, float size)
        {
            Position = position;
            Vector3 halfSize = new Vector3(size / 2);
            Collider = new Collider(position - halfSize, position + halfSize);
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            Position = newPosition;
            // Additional logic for collider update if needed
        }
        public void Update(float deltaTime)
        {
            if (!isDestroyed)
            {
                Position += Velocity * deltaTime;
            }
        }
        public new void Destroy()
        {
            if (!isDestroyed)
            {
                base.Destroy(); // Call base destroy logic

                Console.WriteLine($"Cube at {Collider.Min} - {Collider.Max} is destroyed permanently.");

            }
        }
    }

}

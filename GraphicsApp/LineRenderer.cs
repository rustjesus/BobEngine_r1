using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

public class LineRenderer : IDisposable
{
    private SharpDX.Direct3D11.Device device;
    private DeviceContext context;
    private SharpDX.Direct3D11.Buffer lineVertexBuffer;
    private SharpDX.Direct3D11.Buffer lineConstantBuffer;
    private InputLayout inputLayout;
    private VertexShader vertexShader;
    private PixelShader pixelShader;

    [StructLayout(LayoutKind.Sequential)]
    private struct Vertex
    {
        public Vector3 Position;
        public Vector4 Color;

        public Vertex(Vector3 position, Vector4 color)
        {
            Position = position;
            Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LineConstantBufferData
    {
        public Matrix WorldViewProjection;
    }

    public LineRenderer(SharpDX.Direct3D11.Device device, DeviceContext context)
    {
        this.device = device;
        this.context = context;

        InitializeShaders();
        InitializeBuffers();
    }

    private void InitializeShaders()
    {
        // Navigate to the project root from the bin directory
        var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", ".."));

        // Combine the project root with the Shaders folder
        var shadersDirectory = Path.Combine(projectRoot, "Shaders");

        // Get the full path to the shader file
        var vertexShaderPath = Path.Combine(shadersDirectory, "LineVertexShader.hlsl");
        var pixelShaderPath = Path.Combine(shadersDirectory, "LinePixelShader.hlsl");

        // Compile shaders from HLSL files
        var vertexShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(vertexShaderPath, "main", "vs_5_0");
        vertexShader = new VertexShader(device, vertexShaderByteCode);

        var pixelShaderByteCode = SharpDX.D3DCompiler.ShaderBytecode.CompileFromFile(pixelShaderPath, "main", "ps_5_0");
        pixelShader = new PixelShader(device, pixelShaderByteCode);

        // Define input layout for vertex data
        var inputElements = new[]
        {
            new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
            new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0),
        };

        inputLayout = new InputLayout(device, vertexShaderByteCode, inputElements);
    }

    private void InitializeBuffers()
    {
        // Create a constant buffer for the World-View-Projection matrix specific to lines
        lineConstantBuffer = new SharpDX.Direct3D11.Buffer(device, new BufferDescription
        {
            Usage = ResourceUsage.Default,
            SizeInBytes = Utilities.SizeOf<LineConstantBufferData>(),
            BindFlags = BindFlags.ConstantBuffer,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        });
    }

    private void UpdateLineVertexBuffer(Vertex[] vertices)
    {
        // Check if the buffer needs to be recreated
        if (lineVertexBuffer == null || lineVertexBuffer.Description.SizeInBytes < vertices.Length * Utilities.SizeOf<Vertex>())
        {
            // Dispose of the old buffer
            lineVertexBuffer?.Dispose();

            // Create a new buffer
            lineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);
        }
        else
        {
            // Update the existing buffer
            DataStream stream;
            context.MapSubresource(lineVertexBuffer, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out stream);
            stream.WriteRange(vertices);
            context.UnmapSubresource(lineVertexBuffer, 0);
        }
    }

    public void RenderLines(IEnumerable<(Vector3 Start, Vector3 End, Vector4 ColorStart, Vector4 ColorEnd)> lines, Matrix viewProjectionMatrix)
    {
        var vertices = lines.SelectMany(line => new[]
        {
        new Vertex(line.Start, line.ColorStart),
        new Vertex(line.End, line.ColorEnd)
    }).ToArray();

        UpdateLineVertexBuffer(vertices);

        var wvpMatrix = Matrix.Identity * viewProjectionMatrix;
        var lineConstantBufferData = new LineConstantBufferData
        {
            WorldViewProjection = Matrix.Transpose(wvpMatrix)
        };
        context.UpdateSubresource(ref lineConstantBufferData, lineConstantBuffer);

        context.InputAssembler.InputLayout = inputLayout;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lineVertexBuffer, Utilities.SizeOf<Vertex>(), 0));

        context.VertexShader.Set(vertexShader);
        context.VertexShader.SetConstantBuffer(0, lineConstantBuffer);
        context.PixelShader.Set(pixelShader);

        context.Draw(vertices.Length, 0);

        // Reset state if necessary
        context.VertexShader.Set(null);
        context.PixelShader.Set(null);
    }

    public void RenderWireframeCube(Vector3 center, float size, Matrix viewProjectionMatrix, Vector4 color, Matrix? rotationMatrix = null)
    {
        // Check if center is null
        if (center == null)
        {
            return; // Stop rendering if center is null
        }

        float halfSize = size / 2;

        // Define the 8 corners of the cube
        var corners = new[]
        {
        new Vector3(center.X - halfSize, center.Y - halfSize, center.Z - halfSize), // 0
        new Vector3(center.X + halfSize, center.Y - halfSize, center.Z - halfSize), // 1
        new Vector3(center.X + halfSize, center.Y + halfSize, center.Z - halfSize), // 2
        new Vector3(center.X - halfSize, center.Y + halfSize, center.Z - halfSize), // 3
        new Vector3(center.X - halfSize, center.Y - halfSize, center.Z + halfSize), // 4
        new Vector3(center.X + halfSize, center.Y - halfSize, center.Z + halfSize), // 5
        new Vector3(center.X + halfSize, center.Y + halfSize, center.Z + halfSize), // 6
        new Vector3(center.X - halfSize, center.Y + halfSize, center.Z + halfSize)  // 7
    };

        // Transform corners by the rotation matrix
        for (int i = 0; i < corners.Length; i++)
        {
            if (rotationMatrix.HasValue)
            {
                corners[i] = Vector3.TransformCoordinate(corners[i], rotationMatrix.Value);
            }
        }

        // Indices for the cube edges
        var indices = new[]
        {
        0, 1, 1, 2, 2, 3, 3, 0, // Back face
        4, 5, 5, 6, 6, 7, 7, 4, // Front face
        0, 4, 1, 5, 2, 6, 3, 7  // Connecting edges
    };

        // Create the vertex buffer
        var vertices = indices.Select(i => new Vertex(corners[i], color)).ToArray();

        lineVertexBuffer?.Dispose();
        lineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

        var wvpMatrix = Matrix.Identity * viewProjectionMatrix;
        var lineConstantBufferData = new LineConstantBufferData
        {
            WorldViewProjection = Matrix.Transpose(wvpMatrix)
        };
        context.UpdateSubresource(ref lineConstantBufferData, lineConstantBuffer);

        context.InputAssembler.InputLayout = inputLayout;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lineVertexBuffer, Utilities.SizeOf<Vertex>(), 0));

        context.VertexShader.Set(vertexShader);
        context.VertexShader.SetConstantBuffer(0, lineConstantBuffer);
        context.PixelShader.Set(pixelShader);

        context.Draw(vertices.Length, 0);

        // Reset the state to avoid interfering with other renderers
        context.VertexShader.Set(null);
        context.PixelShader.Set(null);
        context.InputAssembler.InputLayout = null;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList; // Default for cubes
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[0]); // Unbind the vertex buffer safely
    }

    public void UpdateAndRenderMovingLine(Vector3 start, Vector3 end, Matrix viewProjectionMatrix, Vector4 colorStart, Vector4 colorEnd)
    {
        // Update vertices for moving line
        var vertices = new[]
        {
        new Vertex(start, colorStart),
        new Vertex(end, colorEnd)
    };

        // Dispose of the previous vertex buffer if it exists
        lineVertexBuffer?.Dispose();

        // Create a new vertex buffer with updated positions
        lineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

        // Calculate the World-View-Projection (WVP) matrix for the updated positions
        var wvpMatrix = Matrix.Identity * viewProjectionMatrix;
        var lineConstantBufferData = new LineConstantBufferData
        {
            WorldViewProjection = Matrix.Transpose(wvpMatrix)
        };

        // Update constant buffer with new WVP matrix data
        context.UpdateSubresource(ref lineConstantBufferData, lineConstantBuffer);

        // Set input assembler state for line rendering
        context.InputAssembler.InputLayout = inputLayout;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lineVertexBuffer, Utilities.SizeOf<Vertex>(), 0));

        // Set shaders for line rendering
        context.VertexShader.Set(vertexShader);
        context.VertexShader.SetConstantBuffer(0, lineConstantBuffer);
        context.PixelShader.Set(pixelShader);

        // Draw the line with the updated positions
        context.Draw(2, 0);

        // Reset the state to avoid interference with other renderers
        context.VertexShader.Set(null);
        context.PixelShader.Set(null);
        context.InputAssembler.InputLayout = null;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList; // Default for cubes
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[0]); // Unbind the vertex buffer safely
    }
    public void RenderLine(Vector3 start, Vector3 end, Matrix viewProjectionMatrix, Vector4 colorStart, Vector4 colorEnd)
    {
        var vertices = new[]
        {
        new Vertex(start, colorStart),
        new Vertex(end, colorEnd)
        };

        lineVertexBuffer?.Dispose(); 
        
        //Console.WriteLine($"Vertex buffer size: {vertices.Length}");
        //var reason = device.DeviceRemovedReason;
        //Console.WriteLine($"Device Removed Reason: {reason}");

        lineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

        var wvpMatrix = Matrix.Identity * viewProjectionMatrix;
        var lineConstantBufferData = new LineConstantBufferData
        {
            WorldViewProjection = Matrix.Transpose(wvpMatrix)
        };
        context.UpdateSubresource(ref lineConstantBufferData, lineConstantBuffer);

        // Set input assembler state for line rendering
        context.InputAssembler.InputLayout = inputLayout;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lineVertexBuffer, Utilities.SizeOf<Vertex>(), 0));

        // Set shaders for line rendering
        context.VertexShader.Set(vertexShader);
        context.VertexShader.SetConstantBuffer(0, lineConstantBuffer);
        context.PixelShader.Set(pixelShader);

        // Draw the line
        context.Draw(2, 0);

        // Reset the state to avoid interfering with other renderers
        context.VertexShader.Set(null);
        context.PixelShader.Set(null);
        context.InputAssembler.InputLayout = null;
        context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList; // Default for cubes
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding[0]); // Unbind the vertex buffer safely
    }
    public void RenderWireFrames(Vector3 posA, Vector3 posB, Matrix viewProjectionMatrix, Vector4 colorA, Vector4 colorB)
    {
        // Create vertices for the line
        var vertices = new[]
        {
            new Vertex(posA, colorA),
            new Vertex(posB, colorB)
        };

        // Create a separate vertex buffer for lines
        lineVertexBuffer?.Dispose();
        lineVertexBuffer = SharpDX.Direct3D11.Buffer.Create(device, BindFlags.VertexBuffer, vertices);

        // Set the World-View-Projection matrix for the line
        var wvpMatrix = Matrix.Identity * viewProjectionMatrix;
        var lineConstantBufferData = new LineConstantBufferData
        {
            WorldViewProjection = Matrix.Transpose(wvpMatrix)
        };
        context.UpdateSubresource(ref lineConstantBufferData, lineConstantBuffer);

        // Set shaders and buffers
        context.InputAssembler.InputLayout = inputLayout;
        context.InputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.LineList;
        context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(lineVertexBuffer, Utilities.SizeOf<Vertex>(), 0));

        context.VertexShader.Set(vertexShader);
        context.VertexShader.SetConstantBuffer(0, lineConstantBuffer);
        context.PixelShader.Set(pixelShader);

        // Draw the line
        context.Draw(0, 0);
    }

    public void Dispose()
    {
        lineVertexBuffer?.Dispose();
        lineConstantBuffer?.Dispose();
        inputLayout?.Dispose();
        vertexShader?.Dispose();
        pixelShader?.Dispose();
    }
}

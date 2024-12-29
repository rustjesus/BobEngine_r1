using SharpDX;
using SharpDX.Direct3D11;

public class Light
{
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }
    public float Intensity { get; set; }

    // Constructor to initialize the light properties
    public Light(Vector3 position, Vector3 color, float intensity)
    {
        Position = position;
        Color = color;
        Intensity = intensity;
    }

    // This method updates the light buffer with the light data
    public void SetLightData(DeviceContext context, SharpDX.Direct3D11.Buffer lightBuffer)
    {
        // Prepare light data to be sent to the shader
        LightData lightData = new LightData(Position, Color, Intensity);

        // Update light data in the constant buffer
        context.UpdateSubresource(ref lightData, lightBuffer);
        context.PixelShader.SetConstantBuffer(1, lightBuffer);  // Bind light data to Pixel Shader (buffer slot 1)
    }


}

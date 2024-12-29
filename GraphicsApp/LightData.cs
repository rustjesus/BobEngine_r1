using SharpDX;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 16)]  // Ensure correct packing for GPU
public struct LightData
{
    public Vector3 Position;  // 12 bytes
    public float Padding1;    // 4 bytes (to align to 16 bytes)
    public Vector3 Color;     // 12 bytes
    public float Intensity;   // 4 bytes

    // Constructor to initialize the fields
    public LightData(Vector3 position, Vector3 color, float intensity)
    {
        Position = position;
        Padding1 = 0;  // Set padding explicitly
        Color = color;
        Intensity = intensity;
    }
}
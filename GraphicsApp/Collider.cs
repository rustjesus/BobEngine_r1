using SharpDX;
using System;
using System.Collections.Generic;

namespace GraphicsApp
{
    public class Collider
    {
        public Vector3 Min { get; private set; }
        public Vector3 Max { get; private set; }

        public Collider(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        // Check for AABB collision with another collider
        public bool Intersects(Collider other)
        {
            return (Min.X <= other.Max.X && Max.X >= other.Min.X) &&
                   (Min.Y <= other.Max.Y && Max.Y >= other.Min.Y) &&
                   (Min.Z <= other.Max.Z && Max.Z >= other.Min.Z);
        }
    }

    

    

}

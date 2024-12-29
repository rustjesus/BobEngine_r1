using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsApp
{
    public class CollisionManager
    {
        private List<CubeWithCollider> cubes;
        public static bool destroyed = false;
        public CollisionManager()
        {
            cubes = new List<CubeWithCollider>();
        }

        public void AddCube(CubeWithCollider cube)
        {
            cubes.Add(cube);
        }

        public void CheckCollisions()
        {
            var cubesToRemove = new List<CubeWithCollider>();

            for (int i = 0; i < cubes.Count; i++)
            {
                for (int j = i + 1; j < cubes.Count; j++)
                {
                    if (cubes[i].Collider.Intersects(cubes[j].Collider))
                    {
                        Console.WriteLine($"Collision detected between Cube {i} and Cube {j}");

                        // Mark cubes for removal
                        if (!cubesToRemove.Contains(cubes[i]) && !cubes[i].isDestroyed)
                            cubesToRemove.Add(cubes[i]);
                        if (!cubesToRemove.Contains(cubes[j]) && !cubes[j].isDestroyed)
                            cubesToRemove.Add(cubes[j]);
                    }
                }
            }

            // Remove and destroy cubes after iteration
            foreach (var cube in cubesToRemove)
            {
                cube.Destroy(); // Call cube-specific destroy logic
                cubes.Remove(cube);
                destroyed = true;
            }
        }

        public List<CubeWithCollider> GetActiveCubes()
        {
            // Return only non-destroyed cubes
            return cubes.FindAll(cube => !cube.isDestroyed);
        }
    }
}

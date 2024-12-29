using SharpDX;
using SharpDX.Direct3D11;
using SharpGL.SceneGraph.Cameras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsApp.GameScripts
{
    internal class Projectile
    {  /// <summary>
       /// PROJECTILES
       /// </summary>
        public List<CubeWithCollider> spawnedCubes = new List<CubeWithCollider>();
        private Dictionary<CubeWithCollider, LineRenderer> lineRenderers = new Dictionary<CubeWithCollider, LineRenderer>();
        private float cubeSize = 0.1f;
        private float colliderSize;

        public void SpawnCubeProjectile(CollisionManager collisionManager, Camera camera, Device device)
        {
            // Ensure collisionManager and camera are initialized
            if (collisionManager == null || camera == null)
            {
                Console.WriteLine("CollisionManager or Camera is not initialized.");
                return;
            }

            // Create a new cube projectile from the camera's position
            Vector3 cameraPosition = camera.Position;
            Vector3 cameraDirection = Vector3.Normalize(camera.Target - camera.Position);

            // Offset the spawn position by +5 on the Z-axis
            //cameraPosition.Z += 1;

            // Determine the size and velocity of the new cube projectile

            float projectileSpeed = 3f; // Adjust for desired speed

            // Instantiate a new CubeWithCollider
            var newCube = new CubeWithCollider(device, cameraPosition, cubeSize, collisionManager)
            {
                Velocity = cameraDirection * projectileSpeed
            };

            // Add the new cube to the spawnedCubes list
            spawnedCubes.Add(newCube);

            Console.WriteLine($"Spawned a new cube projectile at position {cameraPosition}, velocity: {newCube.Velocity}.");
        }
        public void UpdateAndRenderProjectiles(float deltaTime, Camera camera, DeviceContext context, LineRenderer lineRenderer)
        {
            colliderSize = cubeSize * 2;

            foreach (var spawnedCube in spawnedCubes)
            {
                if (spawnedCube != null && !spawnedCube.isDestroyed)
                {
                    // Update position based on velocity
                    spawnedCube.Update(deltaTime);

                    // Render the cube
                    var spawnedCubeWorldMatrix = Matrix.Translation(spawnedCube.Position);
                    var spawnedCubeWVP = Matrix.Transpose(spawnedCubeWorldMatrix * camera.GetViewProjection());
                    spawnedCube.UpdateColliderPosition(spawnedCube.Position, colliderSize);
                    spawnedCube.RenderDX11(context, spawnedCubeWVP);
                }
            }
            //set collider gizmo (visible lines) for spawned projectiles (cubes)
            foreach (var spawnedCube in spawnedCubes)
            {
                if (spawnedCube != null && !spawnedCube.isDestroyed)//remove line render if destroyed
                {
                    var greenColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Green color
                    var redColor = new Vector4(1.0f, 0.0f, 0.0f, 1.0f); // red color
                    lineRenderer.RenderWireframeCube(spawnedCube.Position, colliderSize, camera.GetViewProjection(), greenColor);
                    lineRenderer.UpdateAndRenderMovingLine(spawnedCube.Position, new Vector3(0,0,0), camera.GetViewProjection(), greenColor, redColor);
                }
            }
            CleanupDestroyedCubes();
        }

        private void CleanupDestroyedCubes()
        {
            var cubesToRemove = spawnedCubes.Where(cube => cube.isDestroyed).ToList();
            foreach (var cube in cubesToRemove)
            {
                if (lineRenderers.TryGetValue(cube, out var lineRenderer))
                {
                    lineRenderer.Dispose();
                    lineRenderers.Remove(cube);
                }
                spawnedCubes.Remove(cube);
            }
        }

        public void SetProjectileCubeSizes()
        {
            foreach (var cube in spawnedCubes)
            {
                cube.InitializeBuffers();
            }
        }
        /// <summary>
        /// PROJECTILES END
        /// </summary>
    }
}

using SharpDX;
using SharpDX.Direct3D11;
using SharpGL.SceneGraph.Cameras;
using SharpGL.SceneGraph.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsApp.GameScripts
{
    internal class EnemyCubes
    {   /// <summary>
        /// ENEMIES
        /// </summary>
        private Matrix worldViewProjection;
        private float enemySize = 0.2f;
        private float colSize;
        public CubeWithCollider[] cubes;
        public static bool rotateEnemyCubes = true;
        public void InitializeEnemyCubes(Vector3[] cubePos, Device device, CollisionManager collisionManager, int enemyCount)
        {   
            // Initialize the cubes array with 3 elements
            cubePos[0] = new Vector3(-5.0f, 0.0f, 5f);
            cubePos[1] = new Vector3(0f, 0.0f, 5f);
            cubePos[2] = new Vector3(5.0f, 0.0f, 5f);
            cubePos[3] = new Vector3(0f, 2f, 5f);

            cubes = new CubeWithCollider[enemyCount];
            cubes[0] = new CubeWithCollider(device, cubePos[0], enemySize, collisionManager);
            cubes[1] = new CubeWithCollider(device, cubePos[1], enemySize, collisionManager);
            cubes[2] = new CubeWithCollider(device, cubePos[2], enemySize, collisionManager);
            cubes[3] = new CubeWithCollider(device, cubePos[3], enemySize, collisionManager);

            SetEnemyCubeSizes();
        }
        public void SetEnemyCubeSizes()
        {
            foreach (var cube in cubes)
            {
                cube.InitializeBuffers();
            }
        }
        public void EnemyRenderSpin(Vector3[] cubePos, Vector3 worldMatrix, Camera camera, Matrix localRot)
        {

            ///ENEMY RENDER

            // Apply translation to move the cube to the position (cubePos[0])
            var translation1 = Matrix.Translation(cubePos[0]); // Move to position of first cube

            // Combine rotation and translation for the first cube
            var worldMatrix1 = localRot * translation1;

            // Apply camera transformations (worldViewProjection)
            var worldViewProjection1 = Matrix.Transpose(worldMatrix1 * camera.GetViewProjection());

            // Second Cube
            var translation2 = Matrix.Translation(cubePos[1]); // Move to position of second cube
            var worldMatrix2 = localRot * translation2;
            var worldViewProjection2 = Matrix.Transpose(worldMatrix2 * camera.GetViewProjection());

            // Third Cube
            var translation3 = Matrix.Translation(cubePos[2]); // Move to position of third cube
            var worldMatrix3 = localRot * translation3;
            var worldViewProjection3 = Matrix.Transpose(worldMatrix3 * camera.GetViewProjection());


            ///ENEMY RENDER END
        }
        public void RenderEnemyCubes(Matrix localRotation, Matrix worldMatrix, Vector3[] cubePos, DeviceContext context, Camera camera, LineRenderer lineRenderer)
        {
            colSize = enemySize * 2;
            for (int i = 0; i < cubes.Length; i++)
            {
                if (cubes[i] != null)
                {
                    if (cubes[i].isDestroyed)
                    {
                        // Check if the cube has just been destroyed
                        if (!cubes[i].isDestroyedHandled)
                        {
                            UIManager.UpdateEnemiesKilled();
                            cubes[i].isDestroyedHandled = true; // Mark as handled
                        }
                        continue; // Skip rendering destroyed cubes
                    }

                    // Render active cubes
                    var translation = Matrix.Translation(cubePos[i]);
                    worldMatrix = localRotation * translation;

                    worldViewProjection = Matrix.Transpose(worldMatrix * camera.GetViewProjection());

                    cubes[i].UpdateColliderPosition(cubePos[i], colSize);
                    cubes[i].RenderDX11(context, worldViewProjection);
                }
            }

            //set collider gizmo (visible lines) for spawned enemies (cubes)
            for (int i = 0; i < cubes.Length; i++)
            {
                //stop rendering if destroyed
                if (cubes[i] != null && !cubes[i].isDestroyed)
                {
                    var greenColor = new Vector4(0.0f, 1.0f, 0.0f, 1.0f); // Green color
                    lineRenderer.RenderWireframeCube(cubes[i].Position, colSize, camera.GetViewProjection(), greenColor);
                }
            }
        }
        /// <summary>
        /// ENEMIES END
        /// </summary>

    }
}

using SharpDX;
using SharpDX.Direct3D11;
using SharpGL.SceneGraph.Cameras;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GraphicsApp.GameScripts
{
    internal class GameInput
    {
        private bool fireProjectile = false;
        public void ShootGun(Input input, Camera camera, Device device, Projectile projectile, CollisionManager collisionManager)
        {
            // Example usage in the RenderLoop or KeyPress handler
            if (input.IsKeyPressed(Keys.Space))
            {
                if (fireProjectile == false)
                {
                    UIManager.UpdateProjectilesShot();
                    Console.WriteLine($"SPAWNING KEY!");
                    projectile.SpawnCubeProjectile(collisionManager, camera, device);
                    projectile.SetProjectileCubeSizes();
                    fireProjectile = true;
                }
            }
            if (input.IsKeyNotPressed(Keys.Space))
            {
                fireProjectile = false;
            }
        }
        public void CameraMovementInput(Input input, Camera camera)
        {
            // Camera movement controls
            if (input.IsKeyPressed(Keys.S))
            {
                camera.AddMovement(Vector3.UnitZ * -1); // Move backward
            }
            if (input.IsKeyPressed(Keys.W))
            {
                camera.AddMovement(Vector3.UnitZ); // Move forward
            }
            if (input.IsKeyPressed(Keys.Left))
            {
                camera.AddMovement(Vector3.UnitX); // Move left
            }
            if (input.IsKeyPressed(Keys.Right))
            {
                camera.AddMovement(Vector3.UnitX * -1); // Move right
            }
            if (input.IsKeyPressed(Keys.Down))
            {
                camera.AddMovement(Vector3.UnitY * -1); // Move down
            }
            if (input.IsKeyPressed(Keys.Up))
            {
                camera.AddMovement(Vector3.UnitY); // Move up
            }
            if (input.IsKeyPressed(Keys.NumPad6))
            {
                camera.AddRotationYaw(0.1f); // Rotate left
            }
            if (input.IsKeyPressed(Keys.NumPad4))
            {
                camera.AddRotationYaw(-0.1f); // Rotate right
            }
            if (input.IsKeyPressed(Keys.NumPad8))
            {
                camera.AddRotationPitch(0.1f); // Rotate up
            }
            if (input.IsKeyPressed(Keys.NumPad2))
            {
                camera.AddRotationPitch(-0.1f); // Rotate down
            }
        }

    }
}

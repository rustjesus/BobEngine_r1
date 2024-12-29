using SharpDX;
using System;

public class Camera
{
    private Matrix viewMatrix;
    private Matrix projectionMatrix;
    private Vector3 position;
    private Vector3 target;
    private Vector3 up;
    private Vector3 moveDelta = Vector3.Zero;
    private float rotationDeltaYaw = 0f;
    private float rotationDeltaPitch = 0f;

    private float moveSpeed = 0.025f;
    private float rotationSpeed = 0.05f;

    // Public properties to expose position and target
    public Vector3 Position => position;
    public Vector3 Target => target;

    public Camera(Vector3 position, Vector3 target, Vector3 up, float aspectRatio)
    {
        this.position = position;
        this.target = target;
        this.up = up;

        UpdateViewMatrix();
        projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 4, aspectRatio, 0.1f, 100.0f);
    }

    private void UpdateViewMatrix()
    {
        viewMatrix = Matrix.LookAtLH(position, target, up);
    }

    public void UpdateProjection(float aspectRatio)
    {
        projectionMatrix = Matrix.PerspectiveFovLH((float)Math.PI / 4, aspectRatio, 0.1f, 100.0f);
    }

    public Matrix GetViewProjection()
    {
        return viewMatrix * projectionMatrix;
    }

    public void AddMovement(Vector3 movement)
    {
        // Calculate the forward and right directions
        var forward = Vector3.Normalize(target - position); // Direction the camera is facing
        var right = Vector3.Cross(forward, up); // Perpendicular to forward and up (right direction)

        // Apply the local up vector for vertical movement (Y direction)
        var upDirection = Vector3.Normalize(up); // Up direction (Y axis)

        // Only move in the forward, right, and up directions based on the movement vector
        Vector3 moveDirection = (forward * movement.Z) + (right * movement.X) + (upDirection * movement.Y);

        moveDelta += moveDirection * moveSpeed; // Apply move speed scaling
    }


    public void AddRotationYaw(float yaw)
    {
        rotationDeltaYaw += yaw * rotationSpeed;
    }

    public void AddRotationPitch(float pitch)
    {
        rotationDeltaPitch += pitch * rotationSpeed;
    }

    public void Update()
    {
        // Apply movement
        position += moveDelta;
        target += moveDelta;

        // Apply yaw rotation (left/right)
        if (rotationDeltaYaw != 0f)
        {
            var direction = Vector3.Normalize(target - position);
            var rotation = Matrix.RotationAxis(up, rotationDeltaYaw);
            direction = Vector3.TransformCoordinate(direction, rotation);
            target = position + direction;
        }

        // Apply pitch rotation (up/down)
        if (rotationDeltaPitch != 0f)
        {
            var direction = Vector3.Normalize(target - position);
            var right = Vector3.Cross(direction, up); // Right vector
            var rotation = Matrix.RotationAxis(right, rotationDeltaPitch);
            direction = Vector3.TransformCoordinate(direction, rotation);
            target = position + direction;
        }

        // Reset deltas
        moveDelta = Vector3.Zero;
        rotationDeltaYaw = 0f;
        rotationDeltaPitch = 0f;

        // Update the view matrix
        UpdateViewMatrix();
    }
}

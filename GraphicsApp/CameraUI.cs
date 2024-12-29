using System;
using System.Windows.Forms;
using SharpDX;

public class CameraUI
{
    private readonly Form uiForm;
    private readonly Label positionLabel;
    private readonly Label targetLabel;

    public CameraUI(Camera camera)
    {

        // Initialize the form and labels
        uiForm = new Form
        {
            Text = "Camera UI",
            Size = new System.Drawing.Size(300, 200),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedToolWindow
        };

        positionLabel = new Label
        {
            Text = "Position: (0, 0, 0)",
            Location = new System.Drawing.Point(10, 10),
            AutoSize = true
        };

        targetLabel = new Label
        {
            Text = "Target: (0, 0, 0)",
            Location = new System.Drawing.Point(10, 40),
            AutoSize = true
        };

        uiForm.Controls.Add(positionLabel);
        uiForm.Controls.Add(targetLabel);

        // Run the form on a separate thread to avoid blocking the rendering loop
        var uiThread = new System.Threading.Thread(() =>
        {
            Application.Run(uiForm);
        })
        {
            IsBackground = true
        };
        uiThread.Start();
    }

    public void UpdateUI(Camera camera)
    {
        // Update UI labels based on camera's position and target
        var cameraPosition = camera.Position; // Assume this provides a Vector3
        var cameraTarget = camera.Target; // Assume this provides a Vector3

        positionLabel.Invoke(new Action(() =>
        {
            positionLabel.Text = $"Position: ({cameraPosition.X:F2}, {cameraPosition.Y:F2}, {cameraPosition.Z:F2})";
        }));

        targetLabel.Invoke(new Action(() =>
        {
            targetLabel.Text = $"Target: ({cameraTarget.X:F2}, {cameraTarget.Y:F2}, {cameraTarget.Z:F2})";
        }));
    }
}

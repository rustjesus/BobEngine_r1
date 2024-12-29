using System.Collections.Generic;
using System.Windows.Forms;
using System;

namespace GraphicsApp
{
    public class UIForm : Form
    {
        private ComboBox resolutionDropdown;
        private ComboBox renderingApiDropdown; // New ComboBox for rendering API
        private Button closeButton;
        public event Action<UIResolution> ResolutionChanged;
        public event Action<string> RenderingApiChanged; // Event for rendering API change

        public UIForm(List<UIResolution> availableResolutions, UIResolution currentResolution)
        {
            Text = "Graphics Settings";
            Width = 300;
            Height = 250;
            StartPosition = FormStartPosition.CenterScreen;

            // Dropdown for resolution selection
            resolutionDropdown = new ComboBox
            {
                DataSource = availableResolutions,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new System.Drawing.Point(10, 10),
                Width = 250
            };
            resolutionDropdown.SelectedItem = currentResolution;
            resolutionDropdown.SelectedIndexChanged += (s, e) =>
            {
                UIResolution selectedResolution = (UIResolution)resolutionDropdown.SelectedItem;
                ResolutionChanged?.Invoke(selectedResolution);
            };

            // Dropdown for rendering API selection
            renderingApiDropdown = new ComboBox
            {
                Location = new System.Drawing.Point(10, 50),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            renderingApiDropdown.Items.AddRange(new[] { "DirectX 11", "DirectX 9" });
            renderingApiDropdown.SelectedIndex = 0; // Default to DirectX 11
            renderingApiDropdown.SelectedIndexChanged += (s, e) =>
            {
                string selectedApi = (string)renderingApiDropdown.SelectedItem;
                RenderingApiChanged?.Invoke(selectedApi);
            };

            // Close button
            closeButton = new Button
            {
                Text = "Close",
                Location = new System.Drawing.Point(10, 100),
                Width = 100
            };
            closeButton.Click += (s, e) => Close();

            Controls.Add(resolutionDropdown);
            Controls.Add(renderingApiDropdown);
            Controls.Add(closeButton);
        }
    }

    public class UIResolution
    {
        public int Width { get; }
        public int Height { get; }

        public UIResolution(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override string ToString()
        {
            return $"{Width}x{Height}";
        }
    }
}

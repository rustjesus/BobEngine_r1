using System;
using System.Windows.Forms;
using System.Drawing;

namespace GraphicsApp
{
    public class UIManager
    {
        public static int projectilesShot = 0;
        private static Label bulletsLabel;
        public static int fpsInt = 0;
        private static Label fpsLabel;
        public static int enemyKills = 0;
        private static Label enemyKillsLabel;
        private static DateTime lastFrameTime = DateTime.Now;
        private static int frameCount = 0;

        public static void CreateUI(Form form)
        {

            //fps label
            fpsLabel = new Label
            {
                Text = "FPS: " + fpsInt.ToString(),
                Location = new Point(10, 5),
                AutoSize = true
            };
            // Add the label to the provided form
            form.Controls.Add(fpsLabel);

            //bullets label
            bulletsLabel = new Label
            {
                Text = "Bullets fired: " + projectilesShot.ToString(),
                Location = new Point(10, 25),
                AutoSize = true
            };
            // Add the label to the provided form
            form.Controls.Add(bulletsLabel);

            //enemies killed label
            enemyKillsLabel = new Label
            {
                Text = "Enemies killed: " + enemyKills.ToString(),
                Location = new Point(10, 45),
                AutoSize = true
            };
            // Add the label to the provided form
            form.Controls.Add(enemyKillsLabel);
        }

        public static void UpdateProjectilesShot()
        {
            projectilesShot++;
            if (bulletsLabel != null)
            {
                bulletsLabel.Text = "Bullets fired: " + projectilesShot.ToString();
            }
        }
        public static void UpdateEnemiesKilled()
        {
            enemyKills++;
            if (enemyKillsLabel != null)
            {
                enemyKillsLabel.Text = "Enemies killed: " + enemyKills.ToString();
            }
        }

        public static void CalculateAndUpdateFPS()
        {
            frameCount++;
            var currentTime = DateTime.Now;
            var elapsedTime = (currentTime - lastFrameTime).TotalSeconds;

            if (elapsedTime >= 1.0)
            {
                fpsInt = (int)(frameCount / elapsedTime);
                frameCount = 0;
                lastFrameTime = currentTime;

                if (fpsLabel != null)
                {
                    fpsLabel.Text = "FPS: " + fpsInt.ToString();
                }
            }
        }
    }
}

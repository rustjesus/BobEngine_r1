using System.Collections.Generic;
using System.Windows.Forms;

namespace GraphicsApp
{
    internal class Input
    {
        private HashSet<Keys> keysPressed = new HashSet<Keys>();

        public void KeyDown(KeyEventArgs e)
        {
            keysPressed.Add(e.KeyCode);
        }

        public void KeyUp(KeyEventArgs e)
        {
            keysPressed.Remove(e.KeyCode);
        }

        public bool IsKeyPressed(Keys key)
        {
            return keysPressed.Contains(key);
        }

        // New method to check if a key is NOT pressed
        public bool IsKeyNotPressed(Keys key)
        {
            return !keysPressed.Contains(key);
        }
    }
}

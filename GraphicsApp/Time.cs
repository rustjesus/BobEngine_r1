using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicsApp
{

    public static class Time
    {
        private static readonly DateTime startTime = DateTime.Now;

        public static float GetTime()
        {
            return (float)(DateTime.Now - startTime).TotalSeconds;
        }
    }
}

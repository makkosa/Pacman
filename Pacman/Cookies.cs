using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Pacman
{
    class Cookies
    {
        public Stopwatch RadiusTimer { get; set; } = new Stopwatch();
        private int BlinkTime = 70;

        public Cookies()
        {
            RadiusTimer = Stopwatch.StartNew();
        }

        public float GetRadius()
        {
            int maxFrames = 6;
            float baseRadius = 4f;
            float radiusIncrement = 0.32f;
            int frameNumber = (int)(RadiusTimer.ElapsedMilliseconds / BlinkTime);

            if (frameNumber >= maxFrames * 2)
            {
                RadiusTimer.Reset();
                RadiusTimer.Start();
            }

            if (frameNumber <= maxFrames) return baseRadius + radiusIncrement * frameNumber;
            else return baseRadius + radiusIncrement * (frameNumber - 2 * (frameNumber - maxFrames));
        }

        public static List<Point> Get()
        {
            return new List<Point> { new Point(34, 413), new Point(526, 413), new Point(35, 154), new Point(527, 154) };
        }
    }
}
